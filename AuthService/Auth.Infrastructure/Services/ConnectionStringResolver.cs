using Auth.Application.Common.Interfaces;
using Auth.Domain.Common.Enums;
using Microsoft.Extensions.Configuration;

namespace Auth.Infrastructure.Services
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
                DatabaseContext.Authentication => GetAuthenticationConnectionString(),
                _ => throw new InvalidOperationException($"Database context '{context}' no está soportado")
            };
        }

        public string GetAuthenticationConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no está configurado");
        }
    }
}
