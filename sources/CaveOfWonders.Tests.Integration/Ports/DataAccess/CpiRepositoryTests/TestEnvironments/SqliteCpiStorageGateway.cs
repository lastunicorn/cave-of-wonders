using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests.TestEnvironments;

internal class SqliteCpiStorageGateway : SqliteStorageGatewayBase, ICpiStorageGateway
{
	public SqliteCpiStorageGateway(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public async Task SeedCpisAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken = default)
	{
		await DbContext.Cpis.AddRangeAsync(cpis
			.Select(x => new CpiEntity
			{
				Year = x.Year,
				Value = x.Value
			}), cancellationToken);
	}

	public async Task<List<Cpi>> GetAllCpisAsync(CancellationToken cancellationToken = default)
	{
		return await DbContext.Cpis
			.Select(x => new Cpi
			{
				Year = x.Year,
				Value = x.Value
			})
			.ToListAsync(cancellationToken);
	}
}