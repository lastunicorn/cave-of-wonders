using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class SqliteCpiRepositoryFixture : ISutFixture<ICpiRepository>
{
	private readonly string dbDirectoryPath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}");
	private readonly string dbFilePath;
	private readonly string connectionString;

	private CaveOfWondersDbContext dbContext;

	public ICpiRepository Sut { get; private set; }

	public SqliteCpiRepositoryFixture()
	{
		dbFilePath = Path.Combine(dbDirectoryPath, "test-database.db");
		connectionString = $"Data Source={dbFilePath}";
	}

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(dbDirectoryPath);

		DbContextOptions<CaveOfWondersDbContext> options = new DbContextOptionsBuilder<CaveOfWondersDbContext>()
			.UseSqlite(connectionString)
			.Options;

		dbContext = new CaveOfWondersDbContext(options);
		await dbContext.Database.EnsureCreatedAsync(cancellationToken);

		Sut = new CpiRepository(dbContext);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await dbContext.SaveChangesAsync(cancellationToken);

		await dbContext.DisposeAsync();
		dbContext = null;
		Sut = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		dbContext?.Dispose();
		dbContext = null;
		Sut = null;

		DeleteDatabaseDirectory();
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		dbContext?.Dispose();
		dbContext = null;
		Sut = null;

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

	public override string ToString()
	{
		return "SQLite";
	}
}
