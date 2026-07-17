using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;

[TestEnvironment("SQLite")]
internal class SqliteAverageWageRepositoryEnvironment : ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway>
{
	private readonly SqliteTempDatabase sqliteTempDatabase = new();
	private SqliteAverageWageStorageGateway gateway;

	public IAverageWageRepository Sut { get; private set; }

	public IAverageWageStorageGateway Gateway => gateway;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.OpenAsync(cancellationToken);
		Sut = new AverageWageRepository(sqliteTempDatabase.DbContext);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		gateway = new SqliteAverageWageStorageGateway(sqliteTempDatabase);
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
