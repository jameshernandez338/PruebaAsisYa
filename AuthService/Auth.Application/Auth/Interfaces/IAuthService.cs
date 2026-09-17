using Auth.Application.Auth.Request;
using Auth.Application.Auth.Response;

namespace Auth.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    }
}
