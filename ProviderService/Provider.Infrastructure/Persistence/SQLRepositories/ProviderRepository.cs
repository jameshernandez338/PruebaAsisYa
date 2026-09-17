using Provider.Domain.Common.Enums;
using Provider.Domain.Provider.Entities;
using Provider.Domain.Provider.Interface;
using Provider.Infrastructure.Persistence.Dapper;
using Provider.Infrastructure.Persistence.DTOs.Provider;

namespace Provider.Infrastructure.Persistence.SQLRepositories;

public class ProviderRepository : IProviderRepository
{
    private readonly IContextualDbExecutor _dbExecutor;
    private const DatabaseContext Context = DatabaseContext.Provider;

    public ProviderRepository(IContextualDbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<IEnumerable<ProviderEntity>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT Id, Name, Lat, Lon, Available, Rating FROM Providers WHERE Available = 1";

        var rows = await _dbExecutor.QueryAsync<ProviderDto>(Context, sql, null, null, cancellationToken);

        return rows.Select(row => ProviderEntity.Reconstitute(
                id: row.Id,
                name: row.Name,
                lat: row.Lat,
                lon: row.Lon,
                available: row.Available,
                rating: row.Rating
            )).ToList();
    }

    public async Task<ProviderEntity?> OptimizeAsync(CancellationToken cancellationToken = default)
    {
        // Default simple implementation: return the highest-rated available provider
        var candidates = (await GetAvailableProvidersAsync(cancellationToken)).ToList();
        if (!candidates.Any()) return null;

        var best = candidates.OrderByDescending(p => p.Rating).FirstOrDefault();
        return best;
    }

    public async Task<ProviderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT Id, Name, Lat, Lon, Available, Rating FROM Providers WHERE Id = @Id";

        var row = await _dbExecutor.QuerySingleOrDefaultAsync<ProviderDto>(Context, sql, new { Id = id }, null, cancellationToken);
        if (row == null) return null;

        return ProviderEntity.Reconstitute(
            id: row.Id,
            name: row.Name,
            lat: row.Lat,
            lon: row.Lon,
            available: row.Available,
            rating: row.Rating
        );
    }

    public async Task UpdateProviderAsync(ProviderEntity provider, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE Providers SET Available = @Available WHERE Id = @Id";
        var parameters = new { Available = provider.Available ? 1 : 0, Id = provider.Id };
        await _dbExecutor.ExecuteAsync(Context, sql, parameters, null, cancellationToken);
    }
}
