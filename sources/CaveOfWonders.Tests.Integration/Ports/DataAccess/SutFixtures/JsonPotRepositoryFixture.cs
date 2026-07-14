using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class JsonPotRepositoryFixture : ISutFixture<IPotRepository>
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

	private Database database;

	public IPotRepository Instance { get; private set; }

	public async Task CreateInstanceAsync(CancellationToken cancellationToken = default)
	{
		database = new Database(dbDirectoryPath);
		await database.LoadAsync(cancellationToken);
		Instance = new PotRepository(database);
	}

	public async Task ReleaseInstanceAsync(CancellationToken cancellationToken = default)
	{
		await database.SaveAsync(cancellationToken);

		database = null;
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
		Instance = null;

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);
	}

	public override string ToString()
	{
		return "Json";
	}
}