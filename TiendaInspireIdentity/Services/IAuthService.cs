
using Microsoft.AspNetCore.Identity;
using  TiendaInspireIdentity.Dto;

namespace TiendaInspireIdentity.Services
{
  public interface IAuthService 
    {
        Task<bool> Register(string email, string password);
        Task<ResponseLogin?> Login(string email, string password);

    }
}
