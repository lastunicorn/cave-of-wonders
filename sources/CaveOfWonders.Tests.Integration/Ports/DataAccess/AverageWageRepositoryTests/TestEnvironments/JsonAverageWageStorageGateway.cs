using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;

internal class JsonAverageWageStorageGateway : JsonStorageGatewayBase, IAverageWageStorageGateway
{
	public JsonAverageWageStorageGateway(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedAverageWagesAsync(IEnumerable<AverageWage> averageWages, CancellationToken cancellationToken = default)
	{
		AverageWageRepository averageWageRepository = new(Database);

		foreach (AverageWage averageWage in averageWages)
			averageWageRepository.Add(averageWage);

		return Task.CompletedTask;
	}

	public Task<List<AverageWage>> GetAllAverageWagesAsync(CancellationToken cancellationToken = default)
	{
		List<AverageWage> averageWages = Database.AverageWages.ToList();
		return Task.FromResult(averageWages);
	}
}
