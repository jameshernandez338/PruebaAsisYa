using Auth.Domain.Auth.Entities;
using Auth.Domain.Auth.Interfaces;
using Auth.Domain.Common.Enums;
using Auth.Infrastructure.Persistence.Dapper;
using Auth.Infrastructure.Persistence.DTOs.Auth;

namespace Auth.Infrastructure.Persistence.SQLRepositories.Auth
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Authentication;

        public UserCredentialRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<UserCredentialEntity?> GetCredentialByUserIdAsync(long userId, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT 
                    u.UserId,
                    u.PasswordHash,
                    u.PasswordChangedAt
                FROM UserCredentials u
                WHERE u.UserId = @UserId
            """;

            var dbCredential = await _dbExecutor.QuerySingleOrDefaultAsync<UserCredentialDto>(
                Context,
                sql,
                new { UserId = userId },
                commandType: null,
                cancellationToken: cancellationToken);

            if (dbCredential == null)
                return null;

            return UserCredentialEntity.Reconstitute(
                userId: dbCredential.UserId,
                passwordHash: dbCredential.PasswordHash,
                passwordChangedAt: dbCredential.PasswordChangedAt
            );
        }
    }
}
