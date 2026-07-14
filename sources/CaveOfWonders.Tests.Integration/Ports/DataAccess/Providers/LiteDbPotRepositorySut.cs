using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;

internal class LiteDbPotRepositorySut : ISutFixture<IPotRepository>
{
	private readonly string dbFilePath = Path.Combine(Path.GetTempPath(), $"test-dbContext-{Guid.NewGuid()}");

	private DbContext dbContext;

	public IPotRepository Instance { get; private set; }

	public Task CreateInstanceAsync(CancellationToken cancellationToken = default)
	{
		dbContext = new DbContext(dbFilePath);
		Instance = new PotRepository(dbContext);

		return Task.CompletedTask;
	}

	public Task ReleaseInstanceAsync(CancellationToken cancellationToken = default)
	{
		dbContext.Dispose();
		dbContext = null;
		Instance = null;

		return Task.CompletedTask;
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
		return "LiteDb";
	}
}