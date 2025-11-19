using Microsoft.AspNetCore.Identity;

namespace TiendaInspireIdentity.Models
{
    public class User : IdentityUser
    {
        public string nombre { get; set; }
        public string email { get; set; }
        public string password { get; set; }



    }
}
