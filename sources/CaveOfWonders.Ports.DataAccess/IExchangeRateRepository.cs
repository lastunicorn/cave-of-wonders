using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IExchangeRateRepository
{
	Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs, CancellationToken cancellationToken = default);

	Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default);

	Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default);

	Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default);

	Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month, CancellationToken cancellationToken = default);

	Task<ExchangeRate> Get(CurrencyPair currencyPair, DateOnly date, CancellationToken cancellationToken = default);

	void Add(ExchangeRate exchangeRate);
}