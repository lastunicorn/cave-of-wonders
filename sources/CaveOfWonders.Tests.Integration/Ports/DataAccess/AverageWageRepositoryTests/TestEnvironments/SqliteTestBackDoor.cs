using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
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
		await DbContext.AverageWages.AddRangeAsync(averageWages
			.Select(x => new AverageWageEntity
			{
				Year = x.Year,
				GrossValue = x.GrossValue,
				NetValue = x.NetValue
			}), cancellationToken);
	}

	public async Task<List<AverageWage>> GetAllAverageWagesAsync(CancellationToken cancellationToken = default)
	{
		return await DbContext.AverageWages
			.Select(x => new AverageWage
			{
				Year = x.Year,
				GrossValue = x.GrossValue,
				NetValue = x.NetValue
			})
			.ToListAsync(cancellationToken);
	}
}