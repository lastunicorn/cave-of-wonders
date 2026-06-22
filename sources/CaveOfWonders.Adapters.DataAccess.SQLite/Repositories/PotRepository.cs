using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class PotRepository : IPotRepository
{
    private readonly CaveOfWondersDbContext dbContext;

    public PotRepository(CaveOfWondersDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<Pot> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Pot> pots = dbContext.Pots
            .Include(x => x.Snapshots)
            .Include(x => x.Labels)
            .AsEnumerable()
            .Select(MapToDomain);

        return pots.ToAsyncEnumerable(cancellationToken);
    }

    public async Task<Pot> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PotEntity entity = await dbContext.Pots
            .Include(x => x.Snapshots)
            .Include(x => x.Labels)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null
            ? null
            : MapToDomain(entity);
    }

    public async Task<IEnumerable<PotSnapshot>> GetSnapshots(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive)
    {
        List<PotEntity> entities = await dbContext.Pots
            .Include(x => x.Snapshots)
            .ToListAsync();

        return entities
            .Select(MapToDomain)
            .Where(x => includeInactive || x.IsActive(date))
            .Select(x => GetSnapshot(x, date, dateMatchingMode))
            .Where(x => x != null);
    }

    public IAsyncEnumerable<Pot> GetByPartialIdAsync(string partialPotId, CancellationToken cancellationToken = default)
    {
        string idWithoutDashes = partialPotId.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = dbContext.Pots
            .Include(x => x.Snapshots)
            .Include(x => x.Labels)
            .AsEnumerable()
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase))
            .Select(MapToDomain);

        return pots.ToAsyncEnumerable(cancellationToken);
    }

    public IAsyncEnumerable<Pot> GetByIdOrName(string idOrName, CancellationToken cancellationToken = default)
    {
        string idWithoutDashes = idOrName.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = dbContext.Pots
            .Include(x => x.Snapshots)
            .Include(x => x.Labels)
            .AsEnumerable()
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase) ||
                (x.Name?.Contains(idOrName, StringComparison.InvariantCultureIgnoreCase) ?? false))
            .Select(MapToDomain);

        return pots.ToAsyncEnumerable(cancellationToken);
    }

    public void Add(Pot pot)
    {
        ArgumentNullException.ThrowIfNull(pot);

        PotEntity entity = new()
        {
            Id = pot.Id,
            Name = pot.Name,
            Description = pot.Description,
            DisplayOrder = pot.DisplayOrder,
            StartDate = pot.StartDate,
            EndDate = pot.EndDate,
            Currency = pot.Currency,
            Snapshots = pot.Snapshots
                .Select(x => new PotSnapshotEntity
                {
                    Date = x.Date,
                    Value = x.Value
                })
                .ToList(),
            Labels = pot.Labels
                .Select(x => new PotLabelEntity
                {
                    Label = x
                })
                .ToList()
        };

        dbContext.Pots.Add(entity);
    }

    private static Pot MapToDomain(PotEntity entity)
    {
        Pot pot = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            DisplayOrder = entity.DisplayOrder,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Currency = entity.Currency
        };

        if (entity.Labels != null)
            pot.Labels.AddRange(entity.Labels.Select(x => x.Label));

        if (entity.Snapshots != null)
        {
            IEnumerable<PotSnapshot> potSnapshots = entity.Snapshots
                .Select(x => new PotSnapshot
                {
                    Date = x.Date,
                    Value = x.Value,
                    Pot = pot
                });

            pot.Snapshots.AddRange(potSnapshots);
        }

        return pot;
    }

    private static PotSnapshot GetSnapshot(Pot pot, DateOnly date, DateMatchingMode dateMatchingMode)
    {
        return dateMatchingMode switch
        {
            DateMatchingMode.Exact => pot.Snapshots
                .FirstOrDefault(x => x.Date == date),
            DateMatchingMode.LastAvailable => pot.Snapshots
                .Where(x => x.Date <= date)
                .MaxBy(x => x.Date),
            _ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
        };
    }
}