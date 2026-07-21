using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests.TestEnvironments;

internal class SqliteTestBackDoor : SqliteStorageBackDoorBase, ITestBackDoor
{
	public SqliteTestBackDoor(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public async Task SeedCpisAsync(IEnumerable<Cpi> cpis, CancellationToken cancellationToken = default)
	{
		await DbContext.Cpis.AddRangeAsync(cpis, cancellationToken);
	}

	public async Task<List<Cpi>> GetAllCpisAsync(CancellationToken cancellationToken = default)
	{
		return await DbContext.Cpis.ToListAsync(cancellationToken);
	}
}