using Microsoft.AspNetCore.Identity;
using TiendaInspireIdentity.Dto.UsersDtos.Responses;

namespace TiendaInspireIdentity.Services
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<UserDetailsResponse?> GetUserByIdAsync(string userId);

        Task<IEnumerable<UserResponse>> GetAllUsersAsync();

        Task<bool> DeleteUserByIdAsync(string userId);



    }
}
