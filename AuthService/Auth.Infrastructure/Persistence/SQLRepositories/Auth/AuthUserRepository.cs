using Auth.Domain.Auth.Entities;
using Auth.Domain.Auth.Interfaces;
using Auth.Domain.Common.Enums;
using Auth.Infrastructure.Persistence.Dapper;
using Auth.Infrastructure.Persistence.DTOs.Auth;

namespace Auth.Infrastructure.Persistence.SQLRepositories.Auth
{
    public class AuthUserRepository : IAuthUserRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Authentication;

        public AuthUserRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<AuthUserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive
                FROM Users u
                LEFT JOIN UserRole ur ON u.Id = ur.UserId
                WHERE u.UserName = @UserName
            ";

            var dbUser = await _dbExecutor.QuerySingleOrDefaultAsync<AuthUserDto>(
                Context,
                sql,
                new { UserName = userName },
                commandType: null,
                cancellationToken: cancellationToken);

            if (dbUser == null)
                return null;

            return AuthUserEntity.Reconstitute(
                id: dbUser.Id,
                userName: dbUser.UserName,
                email: dbUser.Email,
                firstName: dbUser.FirstName,
                lastName: dbUser.LastName,
                isActive: dbUser.IsActive
            );
        }

        public async Task<AuthUserEntity?> GetUserByIdAsync(long userId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive
                FROM Users u
                LEFT JOIN UserRole ur ON u.Id = ur.UserId
                WHERE u.Id = @UserId
            ";

            var dbUser = await _dbExecutor.QuerySingleOrDefaultAsync<AuthUserDto>(
                Context,
                sql,
                new { UserId = userId },
                commandType: null,
                cancellationToken: cancellationToken);

            if (dbUser == null)
                return null;

            return AuthUserEntity.Reconstitute(
                id: dbUser.Id,
                userName: dbUser.UserName,
                email: dbUser.Email,
                firstName: dbUser.FirstName,
                lastName: dbUser.LastName,
                isActive: dbUser.IsActive
            );
        }
    }
}

