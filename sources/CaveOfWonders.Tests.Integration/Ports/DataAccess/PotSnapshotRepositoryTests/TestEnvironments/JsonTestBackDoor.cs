using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;

internal class JsonTestBackDoor : JsonStorageBackDoorBase, ITestBackDoor
{
	public JsonTestBackDoor(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default)
	{
		Database.Pots.AddRange(pots);
		return Task.CompletedTask;
	}

	public Task SeedPotSnapshotsAsync(IEnumerable<PotSnapshot> potSnapshots, CancellationToken cancellationToken = default)
	{
		Database.PotSnapshots.AddRange(potSnapshots);
		return Task.CompletedTask;
	}

	public Task<List<PotSnapshot>> GetSnapshotsByPotIdAsync(Guid potId, CancellationToken cancellationToken = default)
	{
		List<PotSnapshot> snapshots = Database.PotSnapshots
			.Where(x => x.Pot.Id == potId)
			.ToList();

		return Task.FromResult(snapshots);
	}
}
