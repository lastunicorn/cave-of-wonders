using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Gateways;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.LiteDbFixtures;

internal class LiteDbPotRepositoryEnvironment : ITestEnvironment<IPotRepository, IPotStorageGateway>
{
	private readonly LiteDbTempDatabase liteDbTempDatabase = new();
	private LiteDbPotStorageGateway gateway;

	public IPotRepository Sut { get; private set; }

	public IPotStorageGateway Gateway => gateway;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await liteDbTempDatabase.OpenAsync(cancellationToken);
		Sut = new PotRepository(liteDbTempDatabase.DbContext);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await liteDbTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		gateway = new LiteDbPotStorageGateway(liteDbTempDatabase);
		await gateway.OpenAsync(cancellationToken);
	}

	public async Task ReleaseGatewayAsync(CancellationToken cancellationToken = default)
	{
		await gateway.CloseAsync(cancellationToken);
		gateway = null;
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		liteDbTempDatabase.Dispose();
		Sut = null;
		gateway = null;

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		liteDbTempDatabase.Dispose();
		Sut = null;
		gateway = null;
	}

	public override string ToString()
	{
		return "LiteDb";
	}
}
