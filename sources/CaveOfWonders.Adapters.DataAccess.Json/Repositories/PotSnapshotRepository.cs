using DustInTheWind.CaveOfWonders.Domain;
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
}
