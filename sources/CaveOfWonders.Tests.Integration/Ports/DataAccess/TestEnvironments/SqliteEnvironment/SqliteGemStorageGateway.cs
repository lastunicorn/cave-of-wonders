using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments.SqliteEnvironment;

internal class SqliteGemStorageGateway : SqliteStorageGateway, IGemStorageGateway
{
	public SqliteGemStorageGateway(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public Task SeedPotAsync(Pot pot, CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(DbContext);
		potRepository.Add(pot);

		return Task.CompletedTask;
	}

	public Task SeedGemsAsync(IEnumerable<Gem> gems, CancellationToken cancellationToken = default)
	{
		GemRepository gemRepository = new(DbContext);

		foreach (Gem gem in gems)
			gemRepository.Add(gem);

		return Task.CompletedTask;
	}

	public Task<List<Gem>> GetAllGemsAsync(CancellationToken cancellationToken = default)
	{
		GemRepository gemRepository = new(DbContext);

		return gemRepository.FindAsync(new GemFilter(), cancellationToken)
			.ToListAsync();
	}
}
