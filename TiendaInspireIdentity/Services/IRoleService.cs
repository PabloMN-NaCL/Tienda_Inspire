using Microsoft.AspNetCore.Identity;

namespace TiendaInspireIdentity.Services
{
    public interface IRoleService
    {
         Task<bool> CreateRoleAsync(string roleName);

         Task<IdentityRole?>  GetRoleByIdAsync(string roleId);
         Task<bool> DeleteRoleASync(string  roleId);

         Task<bool> UpdateRoleAsync(string roleId ,string newRolename);

        Task<List<IdentityRole>> GetAllRolesAsync();



    }
}
