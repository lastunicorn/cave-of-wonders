using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal class JsonGemRepositoryFixture : IGemRepositorySutFixture
{
	private readonly JsonTempDatabase jsonTempDatabase = new();
	private IPotRepository potRepository;

	public IGemRepository Sut { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.OpenAsync(cancellationToken);

		Sut = new GemRepository(jsonTempDatabase.Database);
		potRepository = new PotRepository(jsonTempDatabase.Database);
	}

	public void SeedPot(Pot pot)
	{
		potRepository.Add(pot);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.CloseAsync(cancellationToken);
		potRepository = null;
		Sut = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		jsonTempDatabase.Dispose();
		potRepository = null;
		Sut = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		jsonTempDatabase.Dispose();
		potRepository = null;
		Sut = null;
	}

	public override string ToString()
	{
		return "Json";
	}
}
