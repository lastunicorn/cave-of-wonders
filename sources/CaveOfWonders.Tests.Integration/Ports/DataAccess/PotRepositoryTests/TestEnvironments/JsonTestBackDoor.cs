using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;

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

	public Task<List<Pot>> GetAllPotsAsync(CancellationToken cancellationToken = default)
	{
		List<Pot> pots = Database.Pots.ToList();
		return Task.FromResult(pots);
	}
}