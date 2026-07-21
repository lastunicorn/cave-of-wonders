using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;

internal class JsonTestBackDoor : JsonStorageBackDoorBase, ITestBackDoor
{
	public JsonTestBackDoor(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedAverageWagesAsync(IEnumerable<AverageWage> averageWages, CancellationToken cancellationToken = default)
	{
		Database.AverageWages.AddRange(averageWages);
		return Task.CompletedTask;
	}

	public Task<List<AverageWage>> GetAllAverageWagesAsync(CancellationToken cancellationToken = default)
	{
		List<AverageWage> averageWages = Database.AverageWages.ToList();
		return Task.FromResult(averageWages);
	}
}