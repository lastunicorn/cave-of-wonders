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

	public IAsyncEnumerable<Gem> FindByDateAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
	{
		return dbContext.Gems
			.Include(x => x.Pot)
			.Where(x => x.Pot.Id == potId && DateOnly.FromDateTime(x.Date) == date)
			.AsAsyncEnumerable();
	}

	public IAsyncEnumerable<Gem> GetByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		return dbContext.Gems
			.Include(x => x.Pot)
			.Where(x => x.Pot.Id == potId)
			.AsAsyncEnumerable();
	}

	public async Task<Gem> GetByExternalIdAsync(Guid potId, string gemExternalId, CancellationToken cancellationToken = default)
	{
		return await dbContext.Gems
			.Include(x => x.Pot)
			.FirstOrDefaultAsync(g => g.Pot.Id == potId && g.ExternalId == gemExternalId, cancellationToken);
	}

	public async Task<Gem> GetLatestAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		return await dbContext.Gems
			.Include(x => x.Pot)
			.Where(x => x.Pot.Id == potId)
			.OrderByDescending(x => x.Date)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public IAsyncEnumerable<Gem> FindByMonthAsync(Guid potId, MonthAndYear month, CancellationToken cancellationToken)
	{
		return dbContext.Gems
			.Include(x => x.Pot)
			.Where(x => x.Pot.Id == potId && x.Date.Year == month.Year && x.Date.Month == month.Month)
			.AsAsyncEnumerable();
	}

	public IAsyncEnumerable<Gem> FindAsync(GemFilter filter, CancellationToken cancellationToken = default)
	{
		IQueryable<Gem> query = dbContext.Gems
			.Include(x => x.Pot)
			.AsQueryable();

		if (filter.PotId != null)
			query = query.Where(x => x.Pot.Id == filter.PotId);

		if (filter.StartDate != null)
			query = query.Where(x => x.Date >= filter.StartDate.Value.ToDateTime(TimeOnly.MinValue));

		if (filter.EndDate != null)
			query = query.Where(x => x.Date <= filter.EndDate.Value.ToDateTime(TimeOnly.MaxValue));

		if (filter.Date != null)
			query = query.Where(x => x.Date.Year == filter.Date.Value.Year && x.Date.Month == filter.Date.Value.Month && x.Date.Day == filter.Date.Value.Day);

		if (filter.Month != null)
			query = query.Where(x => x.Date.Year == filter.Month.Value.Year && x.Date.Month == filter.Month.Value.Month);

		if (filter.IncludeCategories?.Count > 0)
			query = query.Where(x => filter.IncludeCategories.Contains(x.Category));

		if (filter.ExcludeCategories?.Count > 0)
			query = query.Where(x => !filter.ExcludeCategories.Contains(x.Category));

		if (filter.ExternalId != null)
			query = query.Where(x => x.ExternalId == filter.ExternalId);

		return query.AsAsyncEnumerable();
	}

	public void Add(Gem gem)
	{
		ArgumentNullException.ThrowIfNull(gem);

		// gem.Pot is expected to already exist in the database (gems are always added
		// against a pot that was fetched or created beforehand). It typically arrives as a
		// detached stub, so attach it as Unchanged rather than letting EF's graph-walk treat
		// it as a new insert. Reuse an already-tracked instance with the same key instead of
		// attaching a second one, since multiple gems in the same batch can each carry their
		// own separate stub for the same pot.
		Pot trackedPot = dbContext.ChangeTracker.Entries<Pot>()
			.Select(x => x.Entity)
			.FirstOrDefault(x => x.Id == gem.Pot.Id);

		if (trackedPot != null)
			gem.Pot = trackedPot;
		else
			dbContext.Attach(gem.Pot);

		dbContext.Gems.Add(gem);
	}

	public void Remove(Gem gem)
	{
		ArgumentNullException.ThrowIfNull(gem);

		Gem entity = dbContext.Gems.Local.FirstOrDefault(x => x.Id == gem.Id);

		if (entity == null)
		{
			entity = new Gem { Id = gem.Id };
			dbContext.Gems.Attach(entity);
		}

		dbContext.Gems.Remove(entity);
	}
}
