using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;

internal sealed class ExchangeRateTracker
{
    private readonly Dictionary<int, (ExchangeRate Domain, ExchangeRateEntity Entity)> tracked = new();
    private readonly List<(ExchangeRate Domain, ExchangeRateEntity Entity)> pendingInserts = new();

    public ExchangeRate GetOrAttach(ExchangeRateEntity entity)
    {
        if (tracked.TryGetValue(entity.Id, out (ExchangeRate Domain, ExchangeRateEntity Entity) existing))
            return existing.Domain;

        ExchangeRate domain = new()
        {
            Date = entity.Date,
            CurrencyPair = entity.CurrencyPair,
            Value = entity.Value
        };

        tracked[entity.Id] = (domain, entity);
        return domain;
    }

    public void TrackNew(ExchangeRate exchangeRate)
    {
        ExchangeRateEntity entity = new()
        {
            Date = exchangeRate.Date,
            CurrencyPair = exchangeRate.CurrencyPair,
            Value = exchangeRate.Value
        };

        pendingInserts.Add((exchangeRate, entity));
    }

    // Called before EF Core's own SaveChangesAsync: registers new entities with the
    // change tracker and copies the current domain values onto their backing entities
    // so EF's snapshot-based change detection picks up the mutation. Values are re-copied
    // here (not just at TrackNew/GetOrAttach time) because the domain object may have been
    // mutated after being tracked but before this save (e.g. a duplicate seen later in the
    // same import batch).
    public void PrepareChanges(DbSet<ExchangeRateEntity> dbSet)
    {
        foreach ((ExchangeRate domain, ExchangeRateEntity entity) in pendingInserts)
        {
            entity.Value = domain.Value;
            dbSet.Add(entity);
        }

        foreach ((ExchangeRate domain, ExchangeRateEntity entity) in tracked.Values)
            entity.Value = domain.Value;
    }

    // Called after EF Core's own SaveChangesAsync: by then, inserted entities have their
    // database-generated Id populated, so they can be indexed for future GetOrAttach calls.
    public void CompleteChanges()
    {
        foreach ((ExchangeRate domain, ExchangeRateEntity entity) in pendingInserts)
            tracked[entity.Id] = (domain, entity);

        pendingInserts.Clear();
    }
}
