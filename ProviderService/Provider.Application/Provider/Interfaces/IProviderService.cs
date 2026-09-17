using Provider.Application.Provider.Requests;
using Provider.Application.Provider.Response;

namespace Provider.Application.Provider.Interfaces;

public interface IProviderService
{
    Task<IEnumerable<ProviderResponse>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default);
    Task<ProviderResponse?> OptimizeAsync(double userLat, double userLon, CancellationToken cancellationToken = default);
    Task<ProviderResponse?> AssignProviderAsync(AssignProviderRequest request, CancellationToken cancellationToken = default);
}
