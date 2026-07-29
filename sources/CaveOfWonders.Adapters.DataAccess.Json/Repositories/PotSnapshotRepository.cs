using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

public class PotSnapshotRepository : IPotSnapshotRepository
{
	private readonly Database database;

	public PotSnapshotRepository(Database database)
	{
		this.database = database ?? throw new ArgumentNullException(nameof(database));
	}

	public Task<IEnumerable<PotSnapshot>> GetLatestAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		IEnumerable<PotSnapshot> query = database.PotSnapshots
			.Where(x => includeInactive || x.Pot.IsActive(date));

		IEnumerable<PotSnapshot> result = dateMatchingMode switch
		{
			DateMatchingMode.Exact => query.Where(x => x.Date == date),
			DateMatchingMode.LastAvailable => query
				.Where(x => x.Date <= date)
				.GroupBy(x => x.Pot.Id)
				.Select(x => x.MaxBy(y => y.Date)),
			_ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
		};

		return Task.FromResult(result);
	}

	public IAsyncEnumerable<PotSnapshot> GetByPotIdAsync(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default)
	{
		IEnumerable<PotSnapshot> potSnapshots = database.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.Where(x => startDate == null || x.Date >= startDate.Value)
			.Where(x => endDate == null || x.Date <= endDate.Value)
			.OrderBy(x => x.Date);

		return potSnapshots.ToAsyncEnumerable(cancellationToken);
	}

	public Task<int> GetCountAsync(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default)
	{
		int count = database.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.Where(x => startDate == null || x.Date >= startDate.Value)
			.Count(x => endDate == null || x.Date <= endDate.Value);

		return Task.FromResult(count);
	}

	public Task<PotSnapshot> GetLatestByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		PotSnapshot latestSnapshot = database.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.MaxBy(x => x.Date);

		return Task.FromResult(latestSnapshot);
	}

	public Task<PotSnapshot> GetLastAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
	{
		PotSnapshot lastSnapshot = database.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.Where(x => x.Date <= date)
			.MaxBy(x => x.Date);

		return Task.FromResult(lastSnapshot);
	}

	public Task<PotSnapshot> GetNextAsync(Guid potId, DateOnly date, CancellationToken cancellationToken = default)
	{
		PotSnapshot nextSnapshot = database.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.Where(x => x.Date >= date)
			.MinBy(x => x.Date);

		return Task.FromResult(nextSnapshot);
	}

	public void Add(PotSnapshot potSnapshot)
	{
		ArgumentNullException.ThrowIfNull(potSnapshot);

		Pot pot = database.Pots.FirstOrDefault(x => x.Id == potSnapshot.Pot.Id);

		if (pot == null)
			throw new ArgumentException($"Pot with id '{potSnapshot.Pot.Id}' was not found.", nameof(potSnapshot));

		potSnapshot.Pot = pot;
		database.PotSnapshots.Add(potSnapshot);
	}

	public void AddRange(IEnumerable<PotSnapshot> potSnapshots)
	{
		ArgumentNullException.ThrowIfNull(potSnapshots);

		foreach (PotSnapshot potSnapshot in potSnapshots)
			Add(potSnapshot);
	}

	public void RemoveByPotId(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null)
	{
		database.PotSnapshots.RemoveAll(x => x.Pot.Id == potId &&
			(startDate == null || x.Date >= startDate.Value) &&
			(endDate == null || x.Date <= endDate.Value));
	}
}
