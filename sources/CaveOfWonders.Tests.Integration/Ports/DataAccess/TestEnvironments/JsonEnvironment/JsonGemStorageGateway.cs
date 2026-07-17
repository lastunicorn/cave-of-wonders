using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments.JsonEnvironment;

internal class JsonGemStorageGateway : JsonStorageGateway, IGemStorageGateway
{
	public JsonGemStorageGateway(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedPotAsync(Pot pot, CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(Database);
		potRepository.Add(pot);

		return Task.CompletedTask;
	}

	public Task SeedGemsAsync(IEnumerable<Gem> gems, CancellationToken cancellationToken = default)
	{
		GemRepository gemRepository = new(Database);

		foreach (Gem gem in gems)
			gemRepository.Add(gem);

		return Task.CompletedTask;
	}

	public async Task<List<Gem>> GetAllGemsAsync(CancellationToken cancellationToken = default)
	{
		await Database.LoadGemsAsync(cancellationToken);
		return Database.Gems.ToList();
	}
}
