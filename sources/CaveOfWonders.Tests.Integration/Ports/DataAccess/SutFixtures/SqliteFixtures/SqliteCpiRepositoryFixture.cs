using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.SqliteFixtures;

internal class SqliteCpiRepositoryFixture : ISutFixture<ICpiRepository>
{
	private readonly SqliteTempDatabase sqliteTempDatabase = new();

	public ICpiRepository Sut { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.OpenAsync(cancellationToken);

		Sut = new CpiRepository(sqliteTempDatabase.DbContext);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.CloseAsync(cancellationToken);
		Sut = null;
	}

	public async Task ResetAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.DisposeAsync();
		Sut = null;
	}

	public void Dispose()
	{
		sqliteTempDatabase.Dispose();
		Sut = null;
	}

	public override string ToString()
	{
		return "SQLite";
	}
}
