using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class GemRepository : IGemRepository
{
    private readonly CaveOfWondersDbContext dbContext;

    public GemRepository(CaveOfWondersDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async IAsyncEnumerable<Gem> FindByDateAsync(Guid potId, DateOnly date, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<GemEntity> entities = await dbContext.Gems
            .Include(x => x.Parameters)
            .Include(x => x.Pot)
            .Where(x => x.PotId == potId && DateOnly.FromDateTime(x.Date) == date)
            .ToListAsync(cancellationToken);

        foreach (GemEntity entity in entities)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return MapToDomain(entity);
        }
    }

    public async IAsyncEnumerable<Gem> GetByPotIdAsync(Guid potId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<GemEntity> entities = await dbContext.Gems
            .Include(x => x.Parameters)
            .Include(x => x.Pot)
            .Where(x => x.PotId == potId)
            .ToListAsync(cancellationToken);

        foreach (GemEntity entity in entities)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return MapToDomain(entity);
        }
    }

    public async Task<Gem> GetByExternalIdAsync(Guid potId, string gemExternalId, CancellationToken cancellationToken)
    {
        GemEntity entity = await dbContext.Gems
            .Include(x => x.Parameters)
            .Include(x => x.Pot)
            .FirstOrDefaultAsync(g => g.PotId == potId && g.ExternalId == gemExternalId, cancellationToken);

        return entity == null ? null : MapToDomain(entity);
    }

    public async IAsyncEnumerable<Gem> FindByMonthAsync(Guid potId, MonthDate month, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<GemEntity> entities = await dbContext.Gems
            .Include(x => x.Parameters)
            .Include(x => x.Pot)
            .Where(x => x.PotId == potId && x.Date.Year == month.Year && x.Date.Month == month.Month)
            .ToListAsync(cancellationToken);

        foreach (GemEntity entity in entities)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return MapToDomain(entity);
        }
    }

    public IAsyncEnumerable<Gem> FindAsync(GemFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<GemEntity> query = dbContext.Gems
            .Include(x => x.Parameters)
            .Include(x => x.Pot)
            .AsQueryable();

        if (filter.PotId != null)
            query = query.Where(x => x.PotId == filter.PotId);

        if (filter.Date != null)
            query = query.Where(x => x.Date.Year == filter.Date.Value.Year && x.Date.Month == filter.Date.Value.Month && x.Date.Day == filter.Date.Value.Day);

        if (filter.Month != null)
            query = query.Where(x => x.Date.Year == filter.Month.Value.Year && x.Date.Month == filter.Month.Value.Month);

        if (filter.Categories?.Count > 0)
            query = query.Where(x => filter.Categories.Contains((GemCategory)x.Category));

        if (filter.ExternalId != null)
            query = query.Where(x => x.ExternalId == filter.ExternalId);

        return query
            .AsAsyncEnumerable()
            .Select(MapToDomain);
    }

    public void Add(Gem gem)
    {
        ArgumentNullException.ThrowIfNull(gem);

        GemEntity entity = new()
        {
            Id = gem.Id,
            ExternalId = gem.ExternalId,
            Date = gem.Date,
            Category = (int)gem.Category,
            Amount = gem.Amount,
            Description = gem.Description,
            PotId = gem.Pot.Id,
            Parameters = gem.Parameters
                .Select(p => new GemParameterEntity
                {
                    Key = p.Key,
                    Value = p.Value
                })
                .ToList()
        };

        dbContext.Gems.Add(entity);
    }

    private static Gem MapToDomain(GemEntity entity)
    {
        Pot pot = entity.Pot == null
            ? null
            : new Pot
            {
                Id = entity.Pot.Id,
                Name = entity.Pot.Name,
                Description = entity.Pot.Description,
                DisplayOrder = entity.Pot.DisplayOrder,
                StartDate = entity.Pot.StartDate,
                EndDate = entity.Pot.EndDate,
                Currency = entity.Pot.Currency
            };

        Gem gem = new()
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Date = entity.Date,
            Category = (GemCategory)entity.Category,
            Amount = entity.Amount,
            Description = entity.Description,
            Pot = pot
        };

        if (entity.Parameters != null)
        {
            foreach (GemParameterEntity param in entity.Parameters)
                gem.Parameters[param.Key] = param.Value;
        }

        return gem;
    }
}