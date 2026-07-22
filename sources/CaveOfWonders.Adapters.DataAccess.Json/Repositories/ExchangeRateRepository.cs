using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly Database database;

    public ExchangeRateRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            exchangeRates = exchangeRates.Where(x => currencyPairs.Contains(x.CurrencyPair));

        return Task.FromResult(exchangeRates);
    }

    public Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        CurrencyPair invertedCurrencyPair = currencyPair.Invert();

        ExchangeRate exchangeRate = database.ExchangeRates
            .Where(x =>
            {
                if (x.Date > date)
                    return false;

                if (x.CurrencyPair == currencyPair)
                    return true;

                if (allowInverted)
                    return x.CurrencyPair == invertedCurrencyPair;

                return false;
            })
            .MaxBy(x => x.Date);

        return Task.FromResult(exchangeRate);
    }

    public Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
        {
            CurrencyPair[] currencyPairsAll = ExpandCurrencyPairs(currencyPairs, allowInverted);
            exchangeRates = exchangeRates.Where(x => currencyPairsAll.Contains(x.CurrencyPair));
        }

        exchangeRates = exchangeRates.Where(x => x.Date <= date);

        IGrouping<DateOnly, ExchangeRate> exchangeRatesByDate = exchangeRates
            .GroupBy(x => x.Date)
            .OrderByDescending(x => x.Key)
            .FirstOrDefault();

        IEnumerable<ExchangeRate> exchangeRatesFinal = exchangeRatesByDate == null
            ? []
            : exchangeRatesByDate;

        return Task.FromResult(exchangeRatesFinal);
    }

    public Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            exchangeRates = exchangeRates.Where(x => currencyPairs.Contains(x.CurrencyPair));

        if (startDate != null)
            exchangeRates = exchangeRates.Where(x => x.Date >= startDate.Value);

        if (endDate != null)
            exchangeRates = exchangeRates.Where(x => x.Date <= endDate.Value);

        exchangeRates = exchangeRates.OrderBy(x => x.Date);

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<ExchangeRate> exchangeRates = database.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            exchangeRates = exchangeRates.Where(x => currencyPairs.Contains(x.CurrencyPair));

        exchangeRates = exchangeRates.Where(x => x.Date.Year == year);

        if (month != null)
            exchangeRates = exchangeRates.Where(x => x.Date.Month == month.Value);

        exchangeRates = exchangeRates.OrderBy(x => x.Date);

        return Task.FromResult(exchangeRates);
    }

    public Task<ExchangeRate> Get(CurrencyPair currencyPair, DateOnly date, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ExchangeRate exchangeRate = database.ExchangeRates
            .FirstOrDefault(x => x.Date == date && x.CurrencyPair == currencyPair);

        return Task.FromResult(exchangeRate);
    }

    public void Add(ExchangeRate exchangeRate)
    {
        database.ExchangeRates.Add(exchangeRate);
    }

    private static CurrencyPair[] ExpandCurrencyPairs(CurrencyPair[] currencyPairs, bool allowInverted)
    {
        if (currencyPairs == null)
            return [];

        if (allowInverted)
        {
            return currencyPairs
                .SelectMany(x => new[] { x, x.Invert() })
                .Distinct()
                .ToArray();
        }
        else
        {
            return currencyPairs
                .Distinct()
                .ToArray();
        }
    }
}