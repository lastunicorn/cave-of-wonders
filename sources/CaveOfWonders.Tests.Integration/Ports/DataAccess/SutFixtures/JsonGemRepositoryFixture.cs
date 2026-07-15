using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class JsonGemRepositoryFixture : IGemRepositorySutFixture
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

	private Database database;
	private IPotRepository potRepository;

	public IGemRepository Instance { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		database = new Database(dbDirectoryPath);
		await database.LoadAsync(cancellationToken);

		Instance = new GemRepository(database);
		potRepository = new PotRepository(database);
	}

	public void SeedPot(Pot pot)
	{
		potRepository.Add(pot);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await database.SaveAsync(cancellationToken);

		database = null;
		potRepository = null;
		Instance = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		database = null;
		potRepository = null;
		Instance = null;

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);
	}

	public override string ToString()
	{
		return "Json";
	}
}
