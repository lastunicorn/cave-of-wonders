using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

[TestEnvironment("SQLite")]
internal class SqliteTestEnvironment : ITestEnvironment<IExchangeRateRepository, ITestBackDoor>
{
	private readonly SqliteTempDatabase sqliteTempDatabase = new();
	private SqliteTestBackDoor backDoor;

	public IExchangeRateRepository Sut { get; private set; }

	public ITestBackDoor BackDoor => backDoor;

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.OpenAsync(cancellationToken);
		Sut = new ExchangeRateRepository(sqliteTempDatabase.DbContext);
	}

	public async Task CloseSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task CreateBackDoorAsync(CancellationToken cancellationToken = default)
	{
		backDoor = new SqliteTestBackDoor(sqliteTempDatabase);
		await backDoor.OpenAsync(cancellationToken);
	}

	public async Task CloseBackDoorAsync(CancellationToken cancellationToken = default)
	{
		await backDoor.CloseAsync(cancellationToken);
		backDoor = null;
	}

	public async Task ResetAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.DisposeAsync();
		Sut = null;
		backDoor = null;
	}

	public void Dispose()
	{
		sqliteTempDatabase.Dispose();
		Sut = null;
		backDoor = null;
	}

	public override string ToString()
	{
		return "SQLite";
	}
}
