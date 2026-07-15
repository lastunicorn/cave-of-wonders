using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.SqliteFixtures;

internal sealed class SqliteTempDatabase : IDisposable, IAsyncDisposable
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");
	private readonly string connectionString;
	
	private CaveOfWondersDbContext dbContext;

	public CaveOfWondersDbContext DbContext => dbContext;

	public SqliteTempDatabase()
	{
		string dbFilePath = Path.Combine(dbDirectoryPath, "test-database.db");
		connectionString = $"Data Source={dbFilePath}";
	}

	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(dbDirectoryPath);

		DbContextOptions<CaveOfWondersDbContext> options = new DbContextOptionsBuilder<CaveOfWondersDbContext>()
			.UseSqlite(connectionString)
			.Options;

		dbContext = new CaveOfWondersDbContext(options);
		await dbContext.Database.EnsureCreatedAsync(cancellationToken);
	}

	public async Task CloseAsync(CancellationToken cancellationToken = default)
	{
		await dbContext.SaveChangesAsync(cancellationToken);

		await dbContext.DisposeAsync();
		dbContext = null;
	}
	
	public void Dispose()
	{
		dbContext?.Dispose();
		dbContext = null;

		DeleteDatabaseDirectory();
	}

	private void DeleteDatabaseDirectory()
	{
		// A pooled connection can keep the .db/-wal/-shm files open behind the scenes even after
		// Dispose, which would otherwise make the directory deletion below fail intermittently.
		SqliteConnection sqliteConnection = new(connectionString);
		SqliteConnection.ClearPool(sqliteConnection);

		if (Directory.Exists(dbDirectoryPath))
			Directory.Delete(dbDirectoryPath, true);
	}

	public async ValueTask DisposeAsync()
	{
		if (dbContext != null)
		{
			await dbContext.DisposeAsync();
			dbContext = null;
		}
		
		DeleteDatabaseDirectory();
	}
}