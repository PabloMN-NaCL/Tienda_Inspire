
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaInspire.Shared.CommonQuerys;
using TiendaInspireIdentity.Data;
using TiendaInspireIdentity.Dto.UsersDtos.Responses;

namespace TiendaInspireIdentity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<IdentityUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Error changing password for user {UserId}: {Error}", userId, error.Description);
                }

                return false;
            }

            _logger.LogInformation("Password changed successfully for user {UserId}.", userId);

            return true;
        }

 
   
        public async Task<UserDetailsResponse?> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Intento de obtener un usuario con ID nulo o vacio.");
                return null;
            }

        
            var user = await _userManager.FindByIdAsync(userId);
           

            if (user == null)
            {
                _logger.LogInformation("Usuario no encontrado con ID: {UserId}", userId);
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDetailsResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnd = user.LockoutEnd,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Roles = roles.ToList() 
            };
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            _logger.LogInformation("Recuperando todos los usuarios.");

            var users = await _userManager.Users.Select(u => new UserResponse
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!
            }).ToListAsync();

            return users;

         
        }

        public async Task<bool> DeleteUserByIdAsync(string userId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);

            if (userToDelete == null)
            {
                _logger.LogWarning("Usuario con ID {UserId} no encontrado.", userId);
                return false;
            }

            
            var result = await _userManager.DeleteAsync(userToDelete);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario con ID {UserId} eliminado exitosamente.", userId);
                return true;
            }
            else
            {
                
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Fallo al eliminar el usuario con ID {UserId}. Errores: {Errors}", userId, errors);
                return false;
            }
        }





    }
}

