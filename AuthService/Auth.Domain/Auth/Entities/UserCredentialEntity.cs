using Auth.Domain.Auth.ValueObjects;

namespace Auth.Domain.Auth.Entities
{
    public sealed class UserCredentialEntity
    {
        public long UserId { get; private set; }
        public HashedPassword PasswordHash { get; private set; }
        public DateTime PasswordChangedAt { get; private set; }

        private UserCredentialEntity(long userId, HashedPassword passwordHash, DateTime passwordChangedAt)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            PasswordChangedAt = passwordChangedAt;
        }

        public static UserCredentialEntity CreateForNewUser(string passwordHash)
        {
            var hashedPasswordVO = HashedPassword.Create(passwordHash);

            return new UserCredentialEntity(
                0, // UserId se asignará en BD
                hashedPasswordVO,
                DateTime.UtcNow
            );
        }

        public static UserCredentialEntity Create(long userId, string passwordHash)
        {
            if (userId <= 0)
                throw new Exceptions.DomainException("UserId inválido");

            var hashedPasswordVO = HashedPassword.Create(passwordHash);

            return new UserCredentialEntity(
                userId,
                hashedPasswordVO,
                DateTime.UtcNow
            );
        }

        public static UserCredentialEntity Reconstitute(
            long userId,
            string passwordHash,
            DateTime passwordChangedAt)
        {
            var hashedPasswordVO = HashedPassword.Create(passwordHash);
            return new UserCredentialEntity(userId, hashedPasswordVO, passwordChangedAt);
        }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = HashedPassword.Create(newPasswordHash);
            PasswordChangedAt = DateTime.UtcNow;
        }
    }
}
