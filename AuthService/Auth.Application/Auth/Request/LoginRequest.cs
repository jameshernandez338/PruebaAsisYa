using System.ComponentModel.DataAnnotations;

namespace Auth.Application.Auth.Request
{
    public sealed record LoginRequest(
        [Required] string UserName,
        [Required] string Password
    );
}
