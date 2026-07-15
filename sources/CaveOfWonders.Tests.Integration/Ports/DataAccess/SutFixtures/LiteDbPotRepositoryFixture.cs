using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

internal class LiteDbPotRepositoryFixture : ISutFixture<IPotRepository>
{
	private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}");

	private DbContext dbContext;

	public IPotRepository Sut { get; private set; }

	public Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		dbContext = new DbContext(dbFilePath);
		Sut = new PotRepository(dbContext);

		return Task.CompletedTask;
	}

	public Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		dbContext.Dispose();
		dbContext = null;
		Sut = null;

		return Task.CompletedTask;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		dbContext?.Dispose();
		dbContext = null;
		Sut = null;

		if (File.Exists(dbFilePath))
			File.Delete(dbFilePath);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		dbContext?.Dispose();
		dbContext = null;
		Sut = null;

		if (File.Exists(dbFilePath))
			File.Delete(dbFilePath);
	}

	public override string ToString()
	{
		return "LiteDb";
	}
}