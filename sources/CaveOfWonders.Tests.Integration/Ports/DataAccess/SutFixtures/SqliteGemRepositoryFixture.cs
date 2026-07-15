using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class SqliteGemRepositoryFixture : IGemRepositorySutFixture
{
	private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}.db");

	private CaveOfWondersDbContext dbContext;
	private IPotRepository potRepository;

	public IGemRepository Instance { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		DbContextOptions<CaveOfWondersDbContext> options = new DbContextOptionsBuilder<CaveOfWondersDbContext>()
			.UseSqlite($"Data Source={dbFilePath}")
			.Options;

		dbContext = new CaveOfWondersDbContext(options);
		await dbContext.Database.EnsureCreatedAsync(cancellationToken);

		Instance = new GemRepository(dbContext);
		potRepository = new PotRepository(dbContext);
	}

	public void SeedPot(Pot pot)
	{
		potRepository.Add(pot);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await dbContext.SaveChangesAsync(cancellationToken);

		await dbContext.DisposeAsync();
		dbContext = null;
		potRepository = null;
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
		potRepository = null;
		Instance = null;

		if (File.Exists(dbFilePath))
			File.Delete(dbFilePath);
	}

	public override string ToString()
	{
		return "SQLite";
	}
}
