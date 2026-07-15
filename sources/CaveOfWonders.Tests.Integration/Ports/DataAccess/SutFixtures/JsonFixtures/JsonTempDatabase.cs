using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal sealed class JsonTempDatabase : IDisposable
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

	private Database database;

	public Database Database => database;

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		database = new Database(dbDirectoryPath);
		await database.LoadAsync(cancellationToken);
	}

	public async Task CloseAsync(CancellationToken cancellationToken = default)
	{
		await database.SaveAsync(cancellationToken);
		database = null;
	}

	public void Dispose()
	{
		database = null;

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);
	}
}
