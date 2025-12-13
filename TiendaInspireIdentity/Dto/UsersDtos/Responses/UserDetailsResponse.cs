namespace TiendaInspireIdentity.Dto.UsersDtos.Responses
{
    public class UserDetailsResponse
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required bool EmailConfirmed { get; set; }
        public required DateTimeOffset? LockoutEnd { get; set; }
        public required bool TwoFactorEnabled { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
