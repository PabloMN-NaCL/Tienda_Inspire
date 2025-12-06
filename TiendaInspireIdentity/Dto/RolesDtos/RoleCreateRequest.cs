using System.ComponentModel.DataAnnotations;

namespace TiendaInspireIdentity.DTOs
{
   
    public class RoleCreateRequest
    {
       public string Name { get; set; } = string.Empty;
    }
}