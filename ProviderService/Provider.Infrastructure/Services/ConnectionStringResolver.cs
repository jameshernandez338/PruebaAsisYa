using Microsoft.Extensions.Configuration;
using Provider.Application.Common.Interfaces;
using Provider.Domain.Common.Enums;

namespace Provider.Infrastructure.Services
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(DatabaseContext context)
        {
            return context switch
            {
                DatabaseContext.Provider => GetProviderConnectionString(),
                _ => throw new InvalidOperationException($"Database context '{context}' no está soportado")
            };
        }

        public string GetProviderConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no está configurado");
        }
    }
}

