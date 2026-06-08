using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs);

    Task<IEnumerable<ExchangeRate>> Get(DateTime date);

    Task<IEnumerable<ExchangeRate>> Get(CurrencyPair currencyPair, List<DateTime> dates);

    Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateTime date, bool allowInverted = false);

    Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateTime date, bool allowInverted = false);

    Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateTime? startDate, DateTime? endDate);

    Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month);

    Task<ExchangeRateImportReport> Import(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken);
}