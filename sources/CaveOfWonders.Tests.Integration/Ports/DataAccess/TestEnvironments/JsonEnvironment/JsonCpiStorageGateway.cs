using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments.JsonEnvironment;

internal class JsonCpiStorageGateway : JsonStorageGateway, ICpiStorageGateway
{
	public JsonCpiStorageGateway(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedCpisAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken = default)
	{
		CpiRepository cpiRepository = new(Database);

		foreach (Cpi cpi in cpis)
			cpiRepository.Add(cpi);

		return Task.CompletedTask;
	}

	public Task<List<Cpi>> GetAllCpisAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Database.CpiRecords.ToList());
	}
}
