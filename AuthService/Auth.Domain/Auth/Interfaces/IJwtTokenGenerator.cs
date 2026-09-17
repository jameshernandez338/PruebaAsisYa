using Auth.Domain.Auth.Entities;

namespace Auth.Domain.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string Generate(AuthUserEntity user);
    }
}
