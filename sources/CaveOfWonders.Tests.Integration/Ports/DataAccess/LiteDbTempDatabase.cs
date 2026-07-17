using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess;

internal sealed class LiteDbTempDatabase : IDisposable
{
	private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");

	private DbContext dbContext;

	public DbContext DbContext => dbContext;

	public Task OpenAsync(CancellationToken cancellationToken = default)
	{
		dbContext = CreateSession();
		return Task.CompletedTask;
	}

	/// <summary>
	/// Opens an additional, independent session over the same database file. Unlike <see cref="OpenAsync"/>,
	/// the returned <see cref="DbContext"/> is not tracked by this instance; the caller owns it and is
	/// responsible for disposing it. Used by storage back doors that must not share the SUT's session.
	/// </summary>
	public Task<DbContext> CreateSessionAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult(CreateSession());
	}

	private DbContext CreateSession()
	{
		return new DbContext(dbFilePath);
	}

	public Task CloseAsync(CancellationToken cancellationToken = default)
	{
		dbContext.Dispose();
		dbContext = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		dbContext?.Dispose();
		dbContext = null;

		if (File.Exists(dbFilePath))
			File.Delete(dbFilePath);
	}
}
