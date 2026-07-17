using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;

internal class SqliteAverageWageStorageGateway : SqliteStorageGatewayBase, IAverageWageStorageGateway
{
	public SqliteAverageWageStorageGateway(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public Task SeedAverageWagesAsync(IEnumerable<AverageWage> averageWages, CancellationToken cancellationToken = default)
	{
		AverageWageRepository averageWageRepository = new(DbContext);

		foreach (AverageWage averageWage in averageWages)
			averageWageRepository.Add(averageWage);

		return Task.CompletedTask;
	}

	public Task<List<AverageWage>> GetAllAverageWagesAsync(CancellationToken cancellationToken = default)
	{
		AverageWageRepository averageWageRepository = new(DbContext);

		return averageWageRepository.GetAllAsync(cancellationToken)
			.ToListAsync();
	}
}
