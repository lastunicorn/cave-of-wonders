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

		IEnumerable<PotSnapshot> potInstances = database.Pots
			.Where(x => includeInactive || x.IsActive(date))
			.Select(x => x.GetSnapshot(date, dateMatchingMode))
			.Where(x => x != null);

		return Task.FromResult(potInstances);
	}

	public IAsyncEnumerable<PotSnapshot> GetByPotIdAsync(Guid potId, DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default)
	{
		Pot pot = database.Pots.FirstOrDefault(x => x.Id == potId);

		IEnumerable<PotSnapshot> potSnapshots = pot == null
			? []
			: pot.Snapshots
				.Where(x => startDate == null || x.Date >= startDate.Value)
				.Where(x => endDate == null || x.Date <= endDate.Value)
				.OrderBy(x => x.Date);

		return potSnapshots.ToAsyncEnumerable(cancellationToken);
	}

	public Task<int> GetCountAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		Pot pot = database.Pots.FirstOrDefault(x => x.Id == potId);

		return Task.FromResult(pot?.Snapshots.Count ?? 0);
	}

	public Task<PotSnapshot> GetLatestByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		Pot pot = database.Pots.FirstOrDefault(x => x.Id == potId);
		PotSnapshot latestSnapshot = pot?.Snapshots.MaxBy(x => x.Date);

		return Task.FromResult(latestSnapshot);
	}

	public void AddRange(IEnumerable<PotSnapshot> potSnapshots)
	{
		ArgumentNullException.ThrowIfNull(potSnapshots);

		foreach (PotSnapshot potSnapshot in potSnapshots)
		{
			Pot pot = database.Pots.FirstOrDefault(x => x.Id == potSnapshot.Pot.Id);

			if (pot == null)
				throw new ArgumentException($"Pot with id '{potSnapshot.Pot.Id}' was not found.", nameof(potSnapshots));

			pot.Snapshots.Add(potSnapshot);
		}
	}

	public void RemoveByPotId(Guid potId)
	{
		Pot pot = database.Pots.FirstOrDefault(x => x.Id == potId);
		pot?.Snapshots.Clear();
	}
}
