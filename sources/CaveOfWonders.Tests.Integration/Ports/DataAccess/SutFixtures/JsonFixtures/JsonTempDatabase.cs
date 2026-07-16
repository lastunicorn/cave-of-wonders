using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.JsonFixtures;

internal sealed class JsonTempDatabase : IDisposable
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

	private Database database;

	public Database Database => database;

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		database = await CreateSessionAsync(cancellationToken);
	}

	/// <summary>
	/// Opens an additional, independent session over the same database directory. Unlike <see cref="OpenAsync"/>,
	/// the returned <c>Database</c> is not tracked by this instance; the caller owns it and is
	/// responsible for saving it. Used by storage gateways that must not share the SUT's session.
	/// </summary>
	public async Task<Database> CreateSessionAsync(CancellationToken cancellationToken = default)
	{
		Database session = new(dbDirectoryPath);
		await session.LoadAsync(cancellationToken);
		return session;
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
