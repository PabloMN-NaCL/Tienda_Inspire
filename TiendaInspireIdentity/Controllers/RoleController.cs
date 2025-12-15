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

    
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDetailResponse>> GetRoleById(string id)
        {
            _logger.LogInformation("Fetching role: {RoleId}", id);

            
            var roleEntity = await _roleService.GetRoleByIdAsync(id);

            if (roleEntity == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", id);
                // Retorna 404 Not Found con ProblemDetails
                return NotFound(new ProblemDetails
                {
                    Title = "Role not found",
                    Detail = $"Role with ID '{id}' was not found.",
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


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequest model)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var success = await _roleService.CreateRoleAsync(model.Name);

            if (success)
            {

                _logger.LogInformation("Nuevo rol '{RoleName}' creado exitosamente.", model.Name);
                return StatusCode(StatusCodes.Status201Created);
            }


            return BadRequest($"No se pudo crear el rol '{model.Name}'. Podría ya existir.");
        }


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

          
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound($"Rol con ID '{id}' no encontrado para actualizar.");
            }

          
            var success = await _roleService.UpdateRoleAsync(id, model.NewName);

            if (success)
            {
                _logger.LogInformation("Rol con ID '{RoleId}' actualizado a '{NewName}'.", id, model.NewName);
                return NoContent();
            }

 
            return BadRequest($"No se pudo actualizar el rol con ID '{id}'.");
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRole(string id)
        {
           
            var success = await _roleService.DeleteRoleASync(id);

            if (success)
            {
                _logger.LogInformation("Rol con ID '{RoleId}' eliminado exitosamente.", id);
                return NoContent(); 
            }

            
            return BadRequest($"No se pudo eliminar el rol con ID '{id}'. Verifique si tiene usuarios asignados.");
        }
    }
}