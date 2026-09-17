using Provider.Domain.Provider.Entities;

namespace Provider.Domain.Provider.Interface
{
    public interface IProviderRepository
    {
        Task<IEnumerable<ProviderEntity>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default);
        Task<ProviderEntity?> OptimizeAsync(CancellationToken cancellationToken = default);
        Task UpdateProviderAsync(ProviderEntity provider, CancellationToken cancellationToken = default);
        Task<ProviderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
