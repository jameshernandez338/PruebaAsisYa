using Auth.Application.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra automáticamente las implementaciones dentro del ensamblado de Application
        /// usando Scrutor. Permite excluir tipos por fragmentos en su nombre.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<IAuthService>()
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
