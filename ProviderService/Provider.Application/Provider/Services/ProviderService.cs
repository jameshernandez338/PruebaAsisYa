using Provider.Application.Provider.Interfaces;
using Provider.Application.Provider.Response;
using Provider.Domain.Provider.Entities;
using Provider.Domain.Provider.Interface;

namespace Provider.Application.Provider.Services;

public class ProviderService : IProviderService
{
    private readonly IProviderRepository _providerRepository;
    private readonly Random _rnd = new();
    // Weight configuration for scoring
    private const double RatingWeight = 2.0;
    private const double DistanceWeight = 0.1;
    private const double EtaWeight = 0.01;

    public ProviderService(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<ProviderResponse?> AssignProviderAsync(Requests.AssignProviderRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) return null;

        // If a provider id is supplied, attempt direct assignment (validate availability)
        if (request.ProviderId.HasValue && request.ProviderId.Value != Guid.Empty)
        {
            var provider = await _providerRepository.GetByIdAsync(request.ProviderId.Value, cancellationToken);
            if (provider == null) return null; // provider not found
            if (!provider.Available) return null; // not available

            // mark unavailable via domain method and persist
            provider.MarkUnavailable();
            await _providerRepository.UpdateProviderAsync(provider, cancellationToken);

            // compute distance/eta if location provided
            var distance = 0.0;
            var eta = 0.0;
            if (request.UserLat.HasValue && request.UserLon.HasValue)
            {
                distance = CalculateDistanceKm(request.UserLat.Value, request.UserLon.Value, provider.Lat, provider.Lon);
                eta = CalculateEtaMinutes(distance);
            }

            return new ProviderResponse(provider.Id, provider.Name, provider.Lat, provider.Lon, provider.Rating, provider.Available, distance, eta);
        }

        // Otherwise, delegate to OptimizeAsync (selects best automatically)
        var lat = request.UserLat ?? 0.0;
        var lon = request.UserLon ?? 0.0;
        return await OptimizeAsync(lat, lon, cancellationToken);
    }

    public async Task<IEnumerable<ProviderResponse>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.GetAvailableProvidersAsync(cancellationToken);
        return providers.Select(p => new ProviderResponse(
            p.Id,
            p.Name,
            p.Lat,
            p.Lon,
            p.Rating,
            p.Available,
            0.0,
            0.0
        )).ToList();
     }

    public async Task<ProviderResponse?> OptimizeAsync(double userLat, double userLon, CancellationToken cancellationToken = default)
    {
        var candidates = (await _providerRepository.GetAvailableProvidersAsync(cancellationToken)).ToList();
        if (!candidates.Any()) return null;

        ProviderResponse? best = null;
        ProviderEntity? bestEntity = null;
        double bestScore = double.MinValue;
        double bestDistance = 0;
        double bestEta = 0;

        foreach (var p in candidates)
        {
            var distance = CalculateDistanceKm(userLat, userLon, p.Lat, p.Lon);
            var eta = CalculateEtaMinutes(distance);
            // Score with configurable weights
            var score = p.Rating * RatingWeight - distance * DistanceWeight - eta * EtaWeight + (p.Available ? 1.0 : 0.0);
            if (score > bestScore)
            {
                bestScore = score;
                bestEntity = p;
                bestDistance = distance;
                bestEta = eta;
            }
        }

        if (bestEntity != null)
        {
            bestEntity.MarkUnavailable();
            await _providerRepository.UpdateProviderAsync(bestEntity, cancellationToken);

            best = new ProviderResponse(bestEntity.Id, bestEntity.Name, bestEntity.Lat, bestEntity.Lon, bestEntity.Rating, bestEntity.Available, bestDistance, bestEta);
        }

        return best;
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula
        const double R = 6371.0; // Earth radius in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(R * c, 2);
    }

    private static double CalculateEtaMinutes(double distanceKm)
    {
        // Assume average speed 40 km/h
        return Math.Round((distanceKm / 40.0) * 60.0, 1);
    }

    private static double ToRadians(double deg) => deg * (Math.PI / 180.0);
}
