using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.SqliteFixtures;

internal class SqliteGemRepositoryFixture : IGemRepositorySutFixture
{
	private readonly SqliteTempDatabase sqliteTempDatabase = new();
	private IPotRepository potRepository;

	public IGemRepository Sut { get; private set; }

	public async Task CreateSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.OpenAsync(cancellationToken);

		Sut = new GemRepository(sqliteTempDatabase.DbContext);
		potRepository = new PotRepository(sqliteTempDatabase.DbContext);
	}

	public void SeedPot(Pot pot)
	{
		potRepository.Add(pot);
	}

	public async Task ReleaseSutAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.CloseAsync(cancellationToken);
		potRepository = null;
		Sut = null;
	}

	public async Task ResetAsync(CancellationToken cancellationToken = default)
	{
		await sqliteTempDatabase.DisposeAsync();
		potRepository = null;
		Sut = null;
	}

	public void Dispose()
	{
		sqliteTempDatabase.Dispose();
		potRepository = null;
		Sut = null;
	}

	public override string ToString()
	{
		return "SQLite";
	}
}
