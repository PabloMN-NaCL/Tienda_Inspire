
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace TiendaInspireIdentity.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<IdentityUser> _userManager;


        public UsersController(ILogger<UsersController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        // 1. Añade [HttpPost] y 2. Cambia el tipo de retorno a Task<IActionResult>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreationRequest request) // 3. Añade [FromBody]
        {
            var user = new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            // Caso 2: Fallo al crear usuario (Usa BadRequest y el mensaje de error)
            if (!result.Succeeded)
            {
                _logger.LogError(
                    "User creation failed: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

                // Devuelve una respuesta 400 Bad Request con los errores
                return BadRequest(new UserCreationResponse
                {
                    Email = request.Email,
                    Message = "User creation failed",
                    Errors = result.Errors.Select(e => e.Description).ToList() // Convertir a Lista para mayor compatibilidad
                });
            }

            // Caso 1: Usuario creado con éxito (Usa Ok u ActionResult, que por defecto sería 200 OK)
            _logger.LogInformation("User created successfully: {Email}", request.Email);

            return Ok(new UserCreationResponse
            {
                Email = request.Email,
                Message = "User created successfully",
                Errors = new List<string>() // Lista vacía para indicar que no hay errores
            });
        }
    }


    //TODO Convertir estos modelos en modelos o DTO

    public record UserCreationResponse()
    {
        public required string Email { get; set; }
        public required string Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }


    };
    public class UserCreationRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    };
}

