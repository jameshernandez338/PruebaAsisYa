using Auth.Application.Common.Interfaces;
using Auth.Domain.Auth.Interfaces;
using Auth.Infrastructure.Auth;
using Auth.Infrastructure.Persistence.Dapper;
using Auth.Infrastructure.Persistence.SQLRepositories.Auth;
using Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra automáticamente las implementaciones dentro del ensamblado de Infrastructure
        /// usando Scrutor.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IContextualDbExecutor, ContextualDbExecutor>();
            services.AddScoped<IConnectionStringResolver, ConnectionStringResolver>();
            services.AddTransient<DbConnectionFactory>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.Scan(scan => scan
                .FromAssemblyOf<AuthUserRepository>()
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
