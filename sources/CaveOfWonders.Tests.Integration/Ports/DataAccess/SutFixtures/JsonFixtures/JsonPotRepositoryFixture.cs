using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal class JsonPotRepositoryFixture : ISutFixture<IPotRepository>
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

	private Database database;

	public IPotRepository Sut { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		database = new Database(dbDirectoryPath);
		await database.LoadAsync(cancellationToken);
		Sut = new PotRepository(database);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await database.SaveAsync(cancellationToken);

		database = null;
		Sut = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		database = null;
		Sut = null;

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		database = null;
		Sut = null;

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);
	}

	public override string ToString()
	{
		return "Json";
	}
}