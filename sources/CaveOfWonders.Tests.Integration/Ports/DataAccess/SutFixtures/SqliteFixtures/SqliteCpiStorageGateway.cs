using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Gateways;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures.SqliteFixtures;

internal class SqliteCpiStorageGateway : SqliteStorageGateway, ICpiStorageGateway
{
	public SqliteCpiStorageGateway(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public Task SeedCpisAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken = default)
	{
		CpiRepository cpiRepository = new(DbContext);

		foreach (Cpi cpi in cpis)
			cpiRepository.Add(cpi);

		return Task.CompletedTask;
	}

	public Task<List<Cpi>> GetAllCpisAsync(CancellationToken cancellationToken = default)
	{
		CpiRepository cpiRepository = new(DbContext);

		return cpiRepository.GetAllAsync(cancellationToken)
			.ToListAsync();
	}
}
