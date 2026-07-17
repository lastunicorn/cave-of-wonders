using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;

[TestEnvironment("Json")]
internal class JsonPotRepositoryEnvironment : ITestEnvironment<IPotRepository, IPotStorageGateway>
{
	private readonly JsonTempDatabase jsonTempDatabase = new();
	private JsonPotStorageGateway gateway;

	public IPotRepository Sut { get; private set; }

	public IPotStorageGateway Gateway => gateway;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.OpenAsync(cancellationToken);
		Sut = new PotRepository(jsonTempDatabase.Database);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		gateway = new JsonPotStorageGateway(jsonTempDatabase);
		await gateway.OpenAsync(cancellationToken);
	}

	public async Task CloseGatewayAsync(CancellationToken cancellationToken = default)
	{
		await gateway.CloseAsync(cancellationToken);
		gateway = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		jsonTempDatabase.Dispose();
		Sut = null;
		gateway = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		jsonTempDatabase.Dispose();
		Sut = null;
		gateway = null;
	}

	public override string ToString()
	{
		return "Json";
	}
}
