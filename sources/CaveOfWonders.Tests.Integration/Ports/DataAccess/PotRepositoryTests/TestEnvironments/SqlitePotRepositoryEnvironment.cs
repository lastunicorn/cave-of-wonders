using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;

[TestEnvironment("SQLite")]
internal class SqlitePotRepositoryEnvironment : ITestEnvironment<IPotRepository, IPotStorageGateway>
{
	private readonly SqliteTempDatabase sqliteTempDatabase = new();
	private SqlitePotStorageGateway gateway;

	public IPotRepository Sut { get; private set; }

	public IPotStorageGateway Gateway => gateway;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.OpenAsync(cancellationToken);
		Sut = new PotRepository(sqliteTempDatabase.DbContext);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		gateway = new SqlitePotStorageGateway(sqliteTempDatabase);
		await gateway.OpenAsync(cancellationToken);
	}

	public async Task CloseGatewayAsync(CancellationToken cancellationToken = default)
	{
		await gateway.CloseAsync(cancellationToken);
		gateway = null;
	}

	public async Task ResetAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.DisposeAsync();
		Sut = null;
		gateway = null;
	}

	public void Dispose()
	{
		sqliteTempDatabase.Dispose();
		Sut = null;
		gateway = null;
	}

	public override string ToString()
	{
		return "SQLite";
	}
}
