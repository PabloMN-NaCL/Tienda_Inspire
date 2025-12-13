

using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaInspire.Shared;
using TiendaInspireIdentity.Dto;

namespace TiendaInspireIdentity.Services
{
    public class AuthService:IAuthService
    {
        private UserManager<IdentityUser> _userManager;
        
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<IdentityUser> userManager,
           
            IConfiguration configuration,
            IPublishEndpoint publishEndpoint)

        {
            _userManager = userManager;
         
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<ResponseLogin?> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Claim - agregar roles al token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "NoRole")

                 
            };

            //Asignar roles que tiene el usuario
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }



            var secretKey = _configuration["JWT:SecretKey"];
            var audience = _configuration["JWT:Audience"];
            var issuer = _configuration["JWT:Issuer"];
            var expirationMinutes = int.Parse(_configuration["JWT:ExpiryInMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            var encryptedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new ResponseLogin
            {
                Token = encryptedToken,
                ExpirationAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };

        }

        public async Task<bool> Register(string email, string password)
        {

            var result = await _userManager.CreateAsync(new IdentityUser
            {
                UserName = email.Split("@")[0],
                Email = email
            }, password);

            var user = await _userManager.FindByEmailAsync(email);
            if (result != null)
            {
                //Asignar rol por defecto
                await _userManager.AddToRoleAsync(user, "Customer");

                await _publishEndpoint.Publish(new UserCreatedEvents(user.Id, user.Email!));
            }
                

            return false;

        }

    }
}
