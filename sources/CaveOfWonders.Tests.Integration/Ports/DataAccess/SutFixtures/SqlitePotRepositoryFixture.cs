using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class SqlitePotRepositoryFixture : ISutFixture<IPotRepository>
{
	private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}.db");

	private CaveOfWondersDbContext dbContext;

	public IPotRepository Instance { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		DbContextOptions<CaveOfWondersDbContext> options = new DbContextOptionsBuilder<CaveOfWondersDbContext>()
			.UseSqlite($"Data Source={dbFilePath}")
			.Options;

		dbContext = new CaveOfWondersDbContext(options);
		await dbContext.Database.EnsureCreatedAsync(cancellationToken);

		Instance = new PotRepository(dbContext);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await dbContext.SaveChangesAsync(cancellationToken);

		await dbContext.DisposeAsync();
		dbContext = null;
		Instance = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		if (File.Exists(dbFilePath))
			File.Delete(dbFilePath);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		dbContext?.Dispose();
		dbContext = null;
		Instance = null;

		if (File.Exists(dbFilePath))
			File.Delete(dbFilePath);
	}

	public override string ToString()
	{
		return "SQLite";
	}
}
