using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TiendaInspireIdentity.Controllers
{
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        public IActionResult Index()
        {
            return View();
        }
    }
}
