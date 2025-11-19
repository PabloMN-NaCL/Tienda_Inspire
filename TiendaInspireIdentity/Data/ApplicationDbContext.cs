using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace  TiendaInspireIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> //INterfaz añadido por nugget
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
