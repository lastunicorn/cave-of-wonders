using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

[TestEnvironment("Json")]
internal class JsonTestEnvironment : ITestEnvironment<IExchangeRateRepository, ITestBackDoor>
{
	private readonly JsonTempDatabase jsonTempDatabase = new();
	private JsonTestBackDoor backDoor;

	public IExchangeRateRepository Sut { get; private set; }

	public ITestBackDoor BackDoor => backDoor;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.OpenAsync(cancellationToken);
		Sut = new ExchangeRateRepository(jsonTempDatabase.Database);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateBackDoorAsync(CancellationToken cancellationToken = default)
	{
		backDoor = new JsonTestBackDoor(jsonTempDatabase);
		await backDoor.OpenAsync(cancellationToken);
	}

	public async Task CloseBackDoorAsync(CancellationToken cancellationToken = default)
	{
		await backDoor.CloseAsync(cancellationToken);
		backDoor = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		jsonTempDatabase.Dispose();
		Sut = null;
		backDoor = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		jsonTempDatabase.Dispose();
		Sut = null;
		backDoor = null;
	}

	public override string ToString()
	{
		return "Json";
	}
}