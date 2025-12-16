
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaInspireIdentity.Dto.UsersDtos.Responses;
using TiendaInspireIdentity.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TiendaInspireIdentity.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserService userService, ILogger<AdminController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserResponse>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailsResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Informacion de usuario no encontrado: {UserId}", id);
                return NotFound($"Usuario con ID {id} no encontrado.");
            }

          
            return Ok(user);
        }


        [HttpDelete("users/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _userService.DeleteUserByIdAsync(id);

            if (!success)
            {
     
                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado.");
                }

           
                return BadRequest($"Fallo al eliminar el usuario {id}. Verifique logs.");
            }

            _logger.LogInformation("Usuario con ID {UserId} eliminado por Admin.", id);
            return NoContent(); 
        }
    }
}