using Auth.Domain.Common.Enums;

namespace Auth.Application.Common.Interfaces
{
    public interface IConnectionStringResolver
    {
        string GetConnectionString(DatabaseContext context);
    }
}
