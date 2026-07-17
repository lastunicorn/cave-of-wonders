using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;

[TestEnvironment("Json")]
internal class JsonAverageWageRepositoryEnvironment : ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway>
{
	private readonly JsonTempDatabase jsonTempDatabase = new();
	private JsonAverageWageStorageGateway gateway;

	public IAverageWageRepository Sut { get; private set; }

	public IAverageWageStorageGateway Gateway => gateway;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.OpenAsync(cancellationToken);
		Sut = new AverageWageRepository(jsonTempDatabase.Database);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await jsonTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		gateway = new JsonAverageWageStorageGateway(jsonTempDatabase);
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
