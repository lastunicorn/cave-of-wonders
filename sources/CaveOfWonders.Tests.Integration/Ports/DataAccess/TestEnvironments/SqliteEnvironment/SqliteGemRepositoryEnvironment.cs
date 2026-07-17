using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments.SqliteEnvironment;

[TestEnvironment("SQLite")]
internal class SqliteGemRepositoryEnvironment : ITestEnvironment<IGemRepository, IGemStorageGateway>
{
	private readonly SqliteTempDatabase sqliteTempDatabase = new();
	private SqliteGemStorageGateway gateway;

	public IGemRepository Sut { get; private set; }

	public IGemStorageGateway Gateway => gateway;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.OpenAsync(cancellationToken);
		Sut = new GemRepository(sqliteTempDatabase.DbContext);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateGatewayAsync(CancellationToken cancellationToken = default)
	{
		gateway = new SqliteGemStorageGateway(sqliteTempDatabase);
		await gateway.OpenAsync(cancellationToken);
	}

	public async Task ReleaseGatewayAsync(CancellationToken cancellationToken = default)
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
