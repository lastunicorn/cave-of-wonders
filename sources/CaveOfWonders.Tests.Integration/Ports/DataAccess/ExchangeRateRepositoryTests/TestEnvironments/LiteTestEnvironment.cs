using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

[TestEnvironment("LiteDb")]
internal class LiteTestEnvironment : ITestEnvironment<IExchangeRateRepository, ITestBackDoor>
{
	private readonly LiteDbTempDatabase liteDbTempDatabase = new();
	private LiteDbTestBackDoor backDoor;

	public IExchangeRateRepository Sut { get; private set; }

	public ITestBackDoor BackDoor => backDoor;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await liteDbTempDatabase.OpenAsync(cancellationToken);
		Sut = new ExchangeRateRepository(liteDbTempDatabase.DbContext);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await liteDbTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateBackDoorAsync(CancellationToken cancellationToken = default)
	{
		backDoor = new LiteDbTestBackDoor(liteDbTempDatabase);
		await backDoor.OpenAsync(cancellationToken);
	}

	public async Task CloseBackDoorAsync(CancellationToken cancellationToken = default)
	{
		await backDoor.CloseAsync(cancellationToken);
		backDoor = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		liteDbTempDatabase.Dispose();
		Sut = null;
		backDoor = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		liteDbTempDatabase.Dispose();
		Sut = null;
		backDoor = null;
	}

	public override string ToString()
	{
		return "LiteDb";
	}
}