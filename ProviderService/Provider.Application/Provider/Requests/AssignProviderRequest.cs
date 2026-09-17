namespace Provider.Application.Provider.Requests;

public sealed record AssignProviderRequest(
    Guid? ProviderId,
    Guid ClientId,
    double? UserLat,
    double? UserLon,
    string? RequestedService,
    DateTimeOffset RequestedAt
);
