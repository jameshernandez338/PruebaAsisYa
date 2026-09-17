using Provider.Domain.Common.Enums;

namespace Provider.Application.Common.Interfaces
{
    public interface IConnectionStringResolver
    {
        string GetConnectionString(DatabaseContext context);
    }
}
