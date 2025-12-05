
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TiendaInspireIdentity.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
     RoleManager<IdentityRole> roleManager,
     UserManager<IdentityUser> userManager,
     ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<bool> CreateRoleAsync(string roleName)
        {

            var existingRole = await _roleManager.FindByNameAsync(roleName);
           

            if (existingRole is not null)
            {
                _logger.LogWarning("Role already exists.");
                return false;
            }
            var role = new IdentityRole(roleName);
            var createResult = await _roleManager.CreateAsync(role);

            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description);
                _logger.LogError("Failed to create role {RoleName}: {Errors}",
                    roleName, string.Join(", ", errors));
                return false;
            }

            _logger.LogInformation("Role created succesfully.");

            return true;
        }

        public async Task<bool> DeleteRoleASync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                _logger.LogWarning("Role doesnt exist. ");
                return false;
            }

            
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                _logger.LogWarning(
                    $"Cannot delete role, Remove all users from this role first.");
            }

            var deleteResult = await _roleManager.DeleteAsync(role);
            if (!deleteResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failure when deleting role.");
                var errors = deleteResult.Errors.Select(e => e.Description);
                return false;
            }

            _logger.LogInformation("Role deleted successfully: ");

            return true;
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
        {


            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
             
                _logger.LogWarning("Rol con ID '{RoleId}' no encontrado.", roleId);
            }
            else
            {
                _logger.LogInformation("Rol '{RoleName}' encontrado con ID '{RoleId}'.", role.Name, roleId);
            }

            return role;
        }

        public async Task<bool> UpdateRoleAsync(string roleId, string newRolename)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                _logger.LogWarning("Role doesnt exist. ");
                return false;
            }

           
            if (role.Name?.Equals(newRolename, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            role.Name = newRolename;


            return true;
        }
        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            
            return await _roleManager.Roles.ToListAsync();
        }

    }
}
