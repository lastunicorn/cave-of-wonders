using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;

internal class SqliteTestBackDoor : SqliteStorageBackDoorBase, ITestBackDoor
{
	public SqliteTestBackDoor(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public async Task SeedAverageWagesAsync(IEnumerable<AverageWage> averageWages, CancellationToken cancellationToken = default)
	{
		await DbContext.AverageWages.AddRangeAsync(averageWages, cancellationToken);
	}

	public async Task<List<AverageWage>> GetAllAverageWagesAsync(CancellationToken cancellationToken = default)
	{
		return await DbContext.AverageWages.ToListAsync(cancellationToken);
	}
}