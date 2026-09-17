using Microsoft.Extensions.DependencyInjection;
using Provider.Application.Common.Interfaces;
using Provider.Infrastructure.Persistence.Dapper;
using Provider.Infrastructure.Persistence.SQLRepositories;
using Provider.Infrastructure.Services;

namespace Provider.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IContextualDbExecutor, ContextualDbExecutor>();
        services.AddScoped<IConnectionStringResolver, ConnectionStringResolver>();
        services.AddTransient<DbConnectionFactory>();

        // Register repositories automatically (classes ending with Repository)
        services.Scan(scan => scan
            .FromAssemblyOf<ProviderRepository>()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
