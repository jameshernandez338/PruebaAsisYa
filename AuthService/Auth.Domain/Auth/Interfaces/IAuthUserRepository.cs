using Auth.Domain.Auth.Entities;

namespace Auth.Domain.Auth.Interfaces
{
    public interface IAuthUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        Task<AuthUserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<AuthUserEntity?> GetUserByIdAsync(long userId, CancellationToken cancellationToken);
    }
}
