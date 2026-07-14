using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class SqlitePotRepositoryFixture : ISutFixture<IPotRepository>
{
	private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-database-{Guid.NewGuid()}.db");

	private CaveOfWondersDbContext dbContext;
	private UnitOfWork unitOfWork;

	public IPotRepository Instance { get; private set; }

	public async Task CreateInstanceAsync(CancellationToken cancellationToken = default)
	{
		DbContextOptions<CaveOfWondersDbContext> options = new DbContextOptionsBuilder<CaveOfWondersDbContext>()
			.UseSqlite($"Data Source={dbFilePath}")
			.Options;

		dbContext = new CaveOfWondersDbContext(options);
		await dbContext.Database.EnsureCreatedAsync(cancellationToken);

		unitOfWork = new UnitOfWork(dbContext);
		Instance = unitOfWork.PotRepository;
	}

	public async Task ReleaseInstanceAsync(CancellationToken cancellationToken = default)
	{
		// IPotRepository.Add only stages changes on the EF Core change tracker; they aren't
		// written to the database until SaveChanges runs, so flush here to mirror how a real
		// caller would go through IUnitOfWork.SaveChangesAsync before the next phase reads back.
		await unitOfWork.SaveChangesAsync(cancellationToken);

		unitOfWork.Dispose();
		unitOfWork = null;
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
		unitOfWork?.Dispose();
		unitOfWork = null;
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
