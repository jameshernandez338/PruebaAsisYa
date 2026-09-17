using Auth.Domain.Auth.Entities;

namespace Auth.Domain.Auth.Interfaces
{
    public interface IUserCredentialRepository
    {
        Task<UserCredentialEntity?> GetCredentialByUserIdAsync(long userId, CancellationToken cancellationToken);
    }
}
