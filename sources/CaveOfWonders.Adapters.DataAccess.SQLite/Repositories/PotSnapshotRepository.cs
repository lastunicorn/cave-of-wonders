using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class PotSnapshotRepository : IPotSnapshotRepository
{
	private readonly CaveOfWondersDbContext dbContext;

	public PotSnapshotRepository(CaveOfWondersDbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<IEnumerable<PotSnapshot>> GetLatestAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default)
	{
		IQueryable<PotSnapshot> query = dbContext.PotSnapshots
			.Include(x => x.Pot)
			.Where(x => includeInactive || (date >= x.Pot.StartDate && (x.Pot.EndDate == null || date <= x.Pot.EndDate)));

		query = dateMatchingMode switch
		{
			DateMatchingMode.Exact => query.Where(x => x.Date == date),
			DateMatchingMode.LastAvailable => query.Where(x => x.Date <= date),
			_ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
		};

		List<PotSnapshot> snapshots = await query.ToListAsync(cancellationToken);

		return dateMatchingMode == DateMatchingMode.LastAvailable
			? snapshots.GroupBy(x => x.Pot.Id).Select(x => x.MaxBy(y => y.Date))
			: snapshots;
	}

	public IAsyncEnumerable<PotSnapshot> GetByPotIdAsync(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default)
	{
		IQueryable<PotSnapshot> query = dbContext.PotSnapshots
			.Include(x => x.Pot)
			.Where(x => x.Pot.Id == potId);

		if (startDate != null)
			query = query.Where(x => x.Date >= startDate.Value);

		if (endDate != null)
			query = query.Where(x => x.Date <= endDate.Value);

		return query
			.OrderBy(x => x.Date)
			.AsAsyncEnumerable();
	}

	public async Task<int> GetCountAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		return await dbContext.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.CountAsync(cancellationToken);
	}

	public async Task<PotSnapshot> GetLatestByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		return await dbContext.PotSnapshots
			.Include(x => x.Pot)
			.Where(x => x.Pot.Id == potId)
			.OrderByDescending(x => x.Date)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public void AddRange(IEnumerable<PotSnapshot> potSnapshots)
	{
		ArgumentNullException.ThrowIfNull(potSnapshots);

		foreach (PotSnapshot potSnapshot in potSnapshots)
		{
			Pot trackedPot = dbContext.ChangeTracker.Entries<Pot>()
				.Select(x => x.Entity)
				.FirstOrDefault(x => x.Id == potSnapshot.Pot.Id);

			potSnapshot.Pot = trackedPot ?? potSnapshot.Pot;

			if (trackedPot == null)
				dbContext.Attach(potSnapshot.Pot);
		}

		dbContext.PotSnapshots.AddRange(potSnapshots);
	}

	public void RemoveByPotId(Guid potId)
	{
		List<PotSnapshot> entities = dbContext.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.ToList();

		dbContext.PotSnapshots.RemoveRange(entities);
	}
}
