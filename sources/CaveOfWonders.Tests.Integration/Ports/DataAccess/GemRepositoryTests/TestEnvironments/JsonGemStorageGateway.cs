using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;

internal class JsonGemStorageGateway : JsonStorageGatewayBase, IGemStorageGateway
{
	public JsonGemStorageGateway(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedPotAsync(Pot pot, CancellationToken cancellationToken = default)
	{
		Database.Pots.Add(pot);
		return Task.CompletedTask;
	}

	public Task SeedGemsAsync(IEnumerable<Gem> gems, CancellationToken cancellationToken = default)
	{
		Database.Gems.AddRange(gems);
		return Task.CompletedTask;
	}

	public async Task<List<Gem>> GetAllGemsAsync(CancellationToken cancellationToken = default)
	{
		await Database.LoadGemsAsync(cancellationToken);
		return Database.Gems.ToList();
	}
}
