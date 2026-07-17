using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

internal class LiteDbTestBackDoor : LiteDbStorageBackDoorBase, ITestBackDoor
{
	public LiteDbTestBackDoor(LiteDbTempDatabase liteDbTempDatabase)
		: base(liteDbTempDatabase)
	{
	}

	public Task SeedExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken = default)
	{
		foreach (ExchangeRate exchangeRate in exchangeRates)
			DbContext.ExchangeRates.Insert(new ExchangeRateDbEntity(exchangeRate));

		return Task.CompletedTask;
	}

	public Task<List<ExchangeRate>> GetAllExchangeRatesAsync(CancellationToken cancellationToken = default)
	{
		List<ExchangeRate> exchangeRates = DbContext.ExchangeRates
			.FindAll()
			.Select(x => new ExchangeRate
			{
				Date = x.Date,
				CurrencyPair = x.CurrencyPair,
				Value = x.Value
			})
			.ToList();

		return Task.FromResult(exchangeRates);
	}
}
