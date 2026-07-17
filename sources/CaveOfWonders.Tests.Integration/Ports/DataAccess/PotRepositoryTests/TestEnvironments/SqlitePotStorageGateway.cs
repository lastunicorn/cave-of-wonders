using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;

internal class SqlitePotStorageGateway : SqliteStorageGatewayBase, IPotStorageGateway
{
	public SqlitePotStorageGateway(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public Task SeedPotsAsync(IEnumerable<Pot> pots, CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(DbContext);

		foreach (Pot pot in pots)
			potRepository.Add(pot);

		return Task.CompletedTask;
	}

	public Task<List<Pot>> GetAllPotsAsync(CancellationToken cancellationToken = default)
	{
		PotRepository potRepository = new(DbContext);

		return potRepository.GetAllAsync(cancellationToken)
			.ToListAsync();
	}
}
