using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class GemRepository : IGemRepository
{
    private readonly DbContext dbContext;

    public GemRepository(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<Gem> GetByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Gem> GetByExternalIdAsync(Guid potId, string gemExternalId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Gem> FindAsync(GemFilter filter, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Gem> FindByDateAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Gem> FindByMonthAsync(Guid potId, MonthDate month, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Add(Gem gem)
    {
        throw new NotImplementedException();
    }

    public void Remove(Gem gem)
    {
        ArgumentNullException.ThrowIfNull(gem);

        dbContext.Gems.Delete(gem.Id);
    }
}
