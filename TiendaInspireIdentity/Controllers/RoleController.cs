using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaInspireIdentity.Services;
using System.Threading.Tasks;

namespace TiendaInspireIdentity.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Authorize]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;

        public RoleController(
            IRoleService roleService,
            ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        // ------------------------------------------------------------------
        // READ: Obtener todos los roles
        // ------------------------------------------------------------------

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<IdentityRole>))]
        public async Task<IActionResult> GetAllRoles()
        {
            _logger.LogInformation("Solicitud para obtener todos los roles.");
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        // ------------------------------------------------------------------
        // READ: Obtener rol por ID
        // ------------------------------------------------------------------

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IdentityRole))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            _logger.LogInformation("Solicitud para obtener rol con ID: {RoleId}", id);
            var role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
            {
                return NotFound($"Rol con ID '{id}' no encontrado.");
            }

            return Ok(role);
        }

        // ------------------------------------------------------------------
        // CREATE: Crear un nuevo rol
        // ------------------------------------------------------------------

        [HttpPost]
        // Solo usuarios con el rol "Admin" pueden crear nuevos roles
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("El nombre del rol no puede estar vacío.");
            }

            var success = await _roleService.CreateRoleAsync(roleName);

            if (success)
            {
                _logger.LogInformation("Nuevo rol '{RoleName}' creado/verificado.", roleName);
                // Retorna 201 Created. Ya que el método no devuelve el objeto creado, 
                // podemos usar CreatedAtAction si GetRoleByIdAsync devuelve IdentityRole 
                // o simplemente Ok/NoContent. Usaremos Created() genérico aquí.
                return StatusCode(StatusCodes.Status201Created);
            }

            return BadRequest($"No se pudo crear el rol '{roleName}'.");
        }

        // ------------------------------------------------------------------
        // UPDATE: Actualizar el nombre de un rol
        // ------------------------------------------------------------------

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] string newRolename)
        {
            if (string.IsNullOrWhiteSpace(newRolename))
            {
                return BadRequest("El nuevo nombre del rol no puede estar vacío.");
            }

            // Primero verificamos si existe para dar una mejor respuesta (404)
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound($"Rol con ID '{id}' no encontrado para actualizar.");
            }

            var success = await _roleService.UpdateRoleAsync(id, newRolename);

            if (success)
            {
                _logger.LogInformation("Rol con ID '{RoleId}' actualizado a '{NewName}'.", id, newRolename);
                return NoContent(); // 204 No Content indica éxito sin cuerpo de respuesta.
            }

            return BadRequest($"No se pudo actualizar el rol con ID '{id}'.");
        }

        // ------------------------------------------------------------------
        // DELETE: Eliminar un rol
        // ------------------------------------------------------------------

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            // Opcional: Primero verificar si existe para devolver 404
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                // Si el servicio no devuelve error al intentar eliminar un no existente, 
                // devolvemos 404 o 204 (dependiendo de la política de idempotencia)
                return NotFound($"Rol con ID '{id}' no encontrado para eliminar.");
            }

            var success = await _roleService.DeleteRoleASync(id);

            if (success)
            {
                _logger.LogInformation("Rol con ID '{RoleId}' eliminado.", id);
                return NoContent(); // 204 No Content
            }

            return BadRequest($"No se pudo eliminar el rol con ID '{id}'.");
        }
    }
}