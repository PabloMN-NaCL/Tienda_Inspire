namespace TiendaInspireIdentity.Dto.UsersDtos.Requests
{
    public class PasswordChangeRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
