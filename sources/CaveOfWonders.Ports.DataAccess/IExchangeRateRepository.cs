using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs);

    Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false);

    Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false);

    Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate);

    Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month);

    /// <summary>
    /// Retrieves the exchange rate for the exact currency pair and date. Returns null if no such record exists.
    /// No date-nearest fallback and no pair inversion are performed.
    /// </summary>
    Task<ExchangeRate> Get(CurrencyPair currencyPair, DateOnly date);

    /// <summary>
    /// For each item, updates the existing record with the same (CurrencyPair, Date) or inserts a new one.
    /// The caller must ensure <paramref name="exchangeRates"/> contains at most one item per (CurrencyPair, Date) pair.
    /// </summary>
    Task AddOrUpdate(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken);
}