using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;

internal class SqliteTestBackDoor : SqliteStorageBackDoorBase, ITestBackDoor
{
	public SqliteTestBackDoor(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public async Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default)
	{
		await DbContext.Pots.AddRangeAsync(pots, cancellationToken);
	}

	public Task<List<PotSnapshot>> GetSnapshotsByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		PotSnapshotRepository potSnapshotRepository = new(DbContext);

		return potSnapshotRepository.GetByPotIdAsync(potId, cancellationToken: cancellationToken)
			.ToListAsync();
	}
}
