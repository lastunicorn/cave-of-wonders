using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IGemRepository
{
    IAsyncEnumerable<Gem> GetByDateAsync(Guid potId, DateTime date, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Gem> FindByDateAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Gem> GetByPotIdAsync(Guid potId, CancellationToken cancellationToken = default);

    Task<Gem> GetByExternalIdAsync(Guid potId, string gemExternalId, CancellationToken cancellationToken);

    IAsyncEnumerable<Gem> FindByMonthAsync(Guid potId, MonthDate month, CancellationToken cancellationToken);

    IAsyncEnumerable<Gem> FindByMonthAndCategoryAsync(MonthDate month, GemCategory category, CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<Gem> FindAsync(GemFilter filter, CancellationToken cancellationToken = default);

    void Add(Gem gem);
}