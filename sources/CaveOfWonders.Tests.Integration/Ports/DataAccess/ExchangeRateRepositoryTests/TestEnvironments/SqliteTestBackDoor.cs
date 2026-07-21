using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

internal class SqliteTestBackDoor : SqliteStorageBackDoorBase, ITestBackDoor
{
	public SqliteTestBackDoor(SqliteTempDatabase sqliteTempDatabase)
		: base(sqliteTempDatabase)
	{
	}

	public async Task SeedExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken = default)
	{
		await DbContext.ExchangeRates.AddRangeAsync(exchangeRates, cancellationToken);
	}

	public async Task<List<ExchangeRate>> GetAllExchangeRatesAsync(CancellationToken cancellationToken = default)
	{
		return await DbContext.ExchangeRates.ToListAsync(cancellationToken);
	}
}