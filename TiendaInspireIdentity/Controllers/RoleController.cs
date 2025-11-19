using Microsoft.AspNetCore.Mvc;

namespace TiendaInspireIdentity.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
