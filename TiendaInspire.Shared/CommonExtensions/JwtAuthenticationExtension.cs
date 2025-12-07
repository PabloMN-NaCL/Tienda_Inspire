using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace TiendaInspire.Shared.CommonExtensions
{
    //Extension desplegable para configuracion de autenticacion JWT en proyectos
    // Utiliza configuracion desde appsettings.json o valores por defecto
    public static class JwtAuthenticationExtensions
    {

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<JwtBearerEvents>? configureEvents = null)
        {
          
            var jwtSecret = configuration["Jwt:Secret"]
                ?? "build-time-secret-key-minimum-32-characters-required-for-hmac-sha256";
            var jwtIssuer = configuration["Jwt:Issuer"]
                ?? "build-time-issuer";
            var jwtAudience = configuration["Jwt:Audience"]
                ?? "build-time-audience";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.Zero, 
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

              
                if (configureEvents != null)
                {
                    options.Events = new JwtBearerEvents();
                    configureEvents(options.Events);
                }
            });

            services.AddAuthorization();

            return services;
        }
    }
}
