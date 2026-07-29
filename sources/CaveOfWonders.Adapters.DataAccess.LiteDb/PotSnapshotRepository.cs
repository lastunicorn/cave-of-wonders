using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Utils;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class PotSnapshotRepository : IPotSnapshotRepository
{
	private readonly DbContext dbContext;

	public PotSnapshotRepository(DbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public Task<IEnumerable<PotSnapshot>> GetLatestAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		IEnumerable<PotSnapshot> potInstances = dbContext.Pots
			.FindAll()
			.Where(x => includeInactive || x.ToDomainEntity().IsActive(date))
			.Select(x =>
			{
				List<PotSnapshot> snapshots = x.ToPotSnapshots(x.ToDomainEntity());

				return dateMatchingMode switch
				{
					DateMatchingMode.Exact => snapshots.FirstOrDefault(y => y.Date == date),
					DateMatchingMode.LastAvailable => snapshots
						.Where(y => y.Date <= date)
						.MaxBy(y => y.Date),
					_ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
				};
			})
			.Where(x => x != null);

		return Task.FromResult(potInstances);
	}

	public IAsyncEnumerable<PotSnapshot> GetByPotIdAsync(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default)
	{
		PotDbEntity potDbEntity = dbContext.Pots.FindById(potId);

		IEnumerable<PotSnapshot> potSnapshots = potDbEntity == null
			? []
			: potDbEntity.ToPotSnapshots(potDbEntity.ToDomainEntity())
				.Where(x => startDate == null || x.Date >= startDate.Value)
				.Where(x => endDate == null || x.Date <= endDate.Value)
				.OrderBy(x => x.Date);

		return potSnapshots.ToAsyncEnumerable(cancellationToken);
	}

	public Task<int> GetCountAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		PotDbEntity potDbEntity = dbContext.Pots.FindById(potId);

		return Task.FromResult(potDbEntity?.Snapshots.Count ?? 0);
	}

	public Task<PotSnapshot> GetLatestByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		PotDbEntity potDbEntity = dbContext.Pots.FindById(potId);
		PotSnapshot latestSnapshot = potDbEntity?
			.ToPotSnapshots(potDbEntity.ToDomainEntity())
			.MaxBy(x => x.Date);

		return Task.FromResult(latestSnapshot);
	}

	public Task<PotSnapshot> GetLastAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
	{
		PotDbEntity potDbEntity = dbContext.Pots.FindById(potId);
		PotSnapshot lastSnapshot = potDbEntity?
			.ToPotSnapshots(potDbEntity.ToDomainEntity())
			.Where(x => x.Date <= date)
			.MaxBy(x => x.Date);

		return Task.FromResult(lastSnapshot);
	}

	public Task<PotSnapshot> GetNextAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
	{
		PotDbEntity potDbEntity = dbContext.Pots.FindById(potId);
		PotSnapshot nextSnapshot = potDbEntity?
			.ToPotSnapshots(potDbEntity.ToDomainEntity())
			.Where(x => x.Date >= date)
			.MinBy(x => x.Date);

		return Task.FromResult(nextSnapshot);
	}

	public void Add(PotSnapshot potSnapshot)
	{
		ArgumentNullException.ThrowIfNull(potSnapshot);

		PotDbEntity potDbEntity = dbContext.Pots.FindById(potSnapshot.Pot.Id);

		if (potDbEntity == null)
			throw new ArgumentException($"Pot with id '{potSnapshot.Pot.Id}' was not found.", nameof(potSnapshot));

		potDbEntity.Snapshots.Add(new PotSnapshotDbEntity
		{
			Date = potSnapshot.Date,
			Value = potSnapshot.Value,
			IsAutomatic = potSnapshot.IsAutomatic
		});

		dbContext.Pots.Update(potDbEntity);
	}

	public void AddRange(IEnumerable<PotSnapshot> potSnapshots)
	{
		ArgumentNullException.ThrowIfNull(potSnapshots);

		foreach (IGrouping<Guid, PotSnapshot> group in potSnapshots.GroupBy(x => x.Pot.Id))
		{
			PotDbEntity potDbEntity = dbContext.Pots.FindById(group.Key);

			if (potDbEntity == null)
				throw new ArgumentException($"Pot with id '{group.Key}' was not found.", nameof(potSnapshots));

			potDbEntity.Snapshots.AddRange(group.Select(x => new PotSnapshotDbEntity
			{
				Date = x.Date,
				Value = x.Value,
				IsAutomatic = x.IsAutomatic
			}));

			dbContext.Pots.Update(potDbEntity);
		}
	}

	public void RemoveByPotId(Guid potId)
	{
		PotDbEntity potDbEntity = dbContext.Pots.FindById(potId);

		if (potDbEntity == null)
			return;

		potDbEntity.Snapshots.Clear();
		dbContext.Pots.Update(potDbEntity);
	}
}
