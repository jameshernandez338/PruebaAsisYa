namespace Provider.Application.Provider.Response;

public sealed record ProviderResponse(
    Guid Id,
    string Name,
    double Lat,
    double Lon,
    double Rating,
    bool Available,
    double DistanceKm,
    double EtaMinutes
);
