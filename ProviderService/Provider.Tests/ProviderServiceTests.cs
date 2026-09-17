using Provider.Application.Provider.Services;
using Provider.Domain.Provider.Entities;
using Provider.Domain.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Provider.Tests;

public class ProviderServiceTests
{
    [Fact]
    public async Task Optimize_Selects_Best_Available()
    {
        // Arrange: create fake repository returning ProviderEntity objects
        var providers = new[] {
            ProviderEntity.Reconstitute(Guid.NewGuid(), "A", 0.0, 0.0, true, 4.9),
            ProviderEntity.Reconstitute(Guid.NewGuid(), "B", 0.0, 0.0, true, 3.0)
        };

        var repo = new FakeProviderRepository(providers);
        var svc = new ProviderService(repo);

        // Act - call service OptimizeAsync with sample user location
        var assigned = await svc.OptimizeAsync(0.0, 0.0, CancellationToken.None);

        // Assert - service selected the best and repository persisted the change
        Assert.NotNull(assigned);
        Assert.Equal("A", assigned!.Name);

        // The repository should have persisted the provider as unavailable
        var available = (await repo.GetAvailableProvidersAsync()).ToList();
        Assert.DoesNotContain(available, p => p.Name == "A");
    }

    private class FakeProviderRepository : IProviderRepository
    {
        private readonly List<ProviderEntity> _providers;

        public FakeProviderRepository(IEnumerable<ProviderEntity> providers)
        {
            _providers = providers.ToList();
        }

        public Task<IEnumerable<ProviderEntity>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<ProviderEntity>>(_providers.Where(p => p.Available));

        public Task<ProviderEntity?> OptimizeAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderEntity?>(_providers.Where(p => p.Available).OrderByDescending(p => p.Rating).FirstOrDefault());

        public Task UpdateProviderAsync(ProviderEntity provider, CancellationToken cancellationToken = default)
        {
            var existing = _providers.FirstOrDefault(p => p.Id == provider.Id);
            if (existing != null)
            {
                if (!provider.Available) existing.MarkUnavailable();
                else existing.MarkAvailable();
            }
            return Task.CompletedTask;
        }

        public Task<ProviderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderEntity?>(_providers.FirstOrDefault(p => p.Id == id));
    }
}
