namespace Auth.Infrastructure.Persistence.DTOs.Auth
{
    public class UserCredentialDto
    {
        public long UserId { get; set; }
        public string PasswordHash { get; set; } = default!;
        public DateTime PasswordChangedAt { get; set; }
    }
}
