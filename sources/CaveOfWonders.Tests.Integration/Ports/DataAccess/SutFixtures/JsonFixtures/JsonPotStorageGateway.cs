using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Gateways;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal class JsonPotStorageGateway : JsonStorageGateway, IPotStorageGateway
{
	public JsonPotStorageGateway(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(Database);

		foreach (Pot pot in pots)
			potRepository.Add(pot);

		return Task.CompletedTask;
	}

	public Task<List<Pot>> GetAllPotsAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Database.Pots.ToList());
	}
}
