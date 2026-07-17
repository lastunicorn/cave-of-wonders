using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;

/// <summary>
/// Back-door access to the storage behind an <c>IExchangeRateRepository</c> under test. Used by Arrange to seed
/// state and by Assert to inspect persisted state, bypassing the SUT's own read/write paths. Each implementation
/// works against the same temporary resource as the SUT, but through its own, independently opened session.
/// </summary>
public interface ITestBackDoor
{
	Task SeedExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken = default);

	Task<List<ExchangeRate>> GetAllExchangeRatesAsync(CancellationToken cancellationToken = default);
}
