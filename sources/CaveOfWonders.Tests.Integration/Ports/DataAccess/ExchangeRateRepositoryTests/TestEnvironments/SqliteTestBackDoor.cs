using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
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
		await DbContext.ExchangeRates.AddRangeAsync(exchangeRates
			.Select(x => new ExchangeRateEntity
			{
				Date = x.Date,
				CurrencyPair = x.CurrencyPair,
				Value = x.Value
			}), cancellationToken);
	}

	public async Task<List<ExchangeRate>> GetAllExchangeRatesAsync(CancellationToken cancellationToken = default)
	{
		List<ExchangeRateEntity> entities = await DbContext.ExchangeRates.ToListAsync(cancellationToken);

		return entities
			.Select(x => new ExchangeRate
			{
				Date = x.Date,
				CurrencyPair = x.CurrencyPair,
				Value = x.Value
			})
			.ToList();
	}
}
