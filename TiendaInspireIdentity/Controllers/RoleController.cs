using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaInspireIdentity.Services;
using TiendaInspireIdentity.DTOs; 
using System.Threading.Tasks;
using TiendaInspireIdentity.Dto.RolesDtos;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoleResponse>))]
        public async Task<IActionResult> GetAllRoles()
        {
            _logger.LogInformation("Solicitud para obtener todos los roles.");

            var roleEntities = await _roleService.GetAllRolesAsync();

           
            var responseDtos = roleEntities.Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            }).ToList();

            return Ok(responseDtos);
        }

        // ------------------------------------------------------------------
        // READ: Obtener rol por ID
        // ------------------------------------------------------------------

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDetailResponse>> GetRoleById(string roleId)
        {
            _logger.LogInformation("Fetching role: {RoleId}", roleId);

            
            var roleEntity = await _roleService.GetRoleByIdAsync(roleId);

            if (roleEntity == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                // Retorna 404 Not Found con ProblemDetails
                return NotFound(new ProblemDetails
                {
                    Title = "Role not found",
                    Detail = $"Role with ID '{roleId}' was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            // Mapeo (manual)
            var responseDto = new RoleDetailResponse
            {
                RoleId = roleEntity.Id,
                Name = roleEntity.Name ?? string.Empty
            };

            return Ok(responseDto);
        }

        // ------------------------------------------------------------------
        // CREATE: Crear un nuevo rol (Utiliza RoleCreateDto)
        // ------------------------------------------------------------------

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequest model)
        {
            // La validación de DTO (Required, StringLength) se maneja automáticamente
            // por [ApiController] si se utiliza [Required] y [StringLength] en el DTO.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // El controlador usa el nombre del DTO y lo pasa al servicio
            var success = await _roleService.CreateRoleAsync(model.Name);

            if (success)
            {
                // Deberíamos devolver 201 Created y la ubicación del recurso, 
                // pero como el servicio solo devuelve bool, nos limitamos a 201.
                _logger.LogInformation("Nuevo rol '{RoleName}' creado exitosamente.", model.Name);
                return StatusCode(StatusCodes.Status201Created);
            }

            // Si success es false, significa que falló la creación (error en BD) 
            // O, según tu servicio, que ya existe. 
            // Devolvemos 400 Bad Request para indicar un problema.
            return BadRequest($"No se pudo crear el rol '{model.Name}'. Podría ya existir.");
        }

        // ------------------------------------------------------------------
        // UPDATE: Actualizar el nombre de un rol (Utiliza RoleUpdateDto)
        // ------------------------------------------------------------------

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Primero, verificamos si existe para dar un 404 claro.
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound($"Rol con ID '{id}' no encontrado para actualizar.");
            }

            // El servicio recibe el nuevo nombre del DTO.
            var success = await _roleService.UpdateRoleAsync(id, model.NewName);

            if (success)
            {
                _logger.LogInformation("Rol con ID '{RoleId}' actualizado a '{NewName}'.", id, model.NewName);
                return NoContent();
            }

            // Si falla, el servicio registra el error (puede ser un problema de concurrencia, etc.)
            return BadRequest($"No se pudo actualizar el rol con ID '{id}'.");
        }

        // ------------------------------------------------------------------
        // DELETE: Eliminar un rol
        // ------------------------------------------------------------------

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            // El servicio se encarga de buscar y verificar si tiene usuarios
            var success = await _roleService.DeleteRoleASync(id);

            if (success)
            {
                _logger.LogInformation("Rol con ID '{RoleId}' eliminado exitosamente.", id);
                return NoContent(); // 204 No Content
            }

            
            return BadRequest($"No se pudo eliminar el rol con ID '{id}'. Verifique si tiene usuarios asignados.");
        }
    }
}