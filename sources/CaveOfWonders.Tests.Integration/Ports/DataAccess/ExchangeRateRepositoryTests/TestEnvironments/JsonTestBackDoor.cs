using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

internal class JsonTestBackDoor : JsonStorageBackDoorBase, ITestBackDoor
{
	public JsonTestBackDoor(JsonTempDatabase jsonTempDatabase)
		: base(jsonTempDatabase)
	{
	}

	public Task SeedExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken = default)
	{
		Database.ExchangeRates.AddRange(exchangeRates);
		return Task.CompletedTask;
	}

	public Task<List<ExchangeRate>> GetAllExchangeRatesAsync(CancellationToken cancellationToken = default)
	{
		List<ExchangeRate> exchangeRates = Database.ExchangeRates.ToList();
		return Task.FromResult(exchangeRates);
	}
}