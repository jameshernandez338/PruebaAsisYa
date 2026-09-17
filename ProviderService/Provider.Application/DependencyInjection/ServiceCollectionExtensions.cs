using Microsoft.Extensions.DependencyInjection;
using Provider.Application.Provider.Services;

namespace Provider.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
               .FromAssemblyOf<ProviderService>()
               .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
               .AsImplementedInterfaces()
               .WithScopedLifetime());

        return services;
    }
}
