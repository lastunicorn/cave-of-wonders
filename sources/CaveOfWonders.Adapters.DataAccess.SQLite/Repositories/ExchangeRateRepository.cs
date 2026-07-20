using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Repositories;

internal class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly CaveOfWondersDbContext dbContext;

    public ExchangeRateRepository(CaveOfWondersDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs, CancellationToken cancellationToken = default)
    {
        IQueryable<ExchangeRate> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            query = query.Where(x => currencyPairs.Contains(x.CurrencyPair));

        return await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);
    }

    public async Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
    {
        CurrencyPair[] pairs = allowInverted
            ? [currencyPair, currencyPair.Invert()]
            : [currencyPair];

        return await dbContext.ExchangeRates
            .Where(x => pairs.Contains(x.CurrencyPair) && x.Date <= date)
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
    {
        DateOnly? maxDate = await dbContext.ExchangeRates
            .Where(x => currencyPairs.Contains(x.CurrencyPair) && x.Date <= date)
            .MaxAsync(x => (DateOnly?)x.Date, cancellationToken);

        if (maxDate == null)
            return Enumerable.Empty<ExchangeRate>();

        return await dbContext.ExchangeRates
            .Where(x => currencyPairs.Contains(x.CurrencyPair) && x.Date == maxDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default)
    {
        IQueryable<ExchangeRate> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            query = query.Where(x => currencyPairs.Contains(x.CurrencyPair));

        if (startDate != null)
            query = query.Where(x => x.Date >= startDate.Value);

        if (endDate != null)
            query = query.Where(x => x.Date <= endDate.Value);

        return await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month, CancellationToken cancellationToken = default)
    {
        IQueryable<ExchangeRate> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
            query = query.Where(x => currencyPairs.Contains(x.CurrencyPair));

        query = query.Where(x => x.Date.Year == (int)year);

        if (month != null)
            query = query.Where(x => x.Date.Month == (int)month.Value);

        return await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);
    }

    public async Task<ExchangeRate> Get(CurrencyPair currencyPair, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await dbContext.ExchangeRates
            .FirstOrDefaultAsync(x => x.Date == date && x.CurrencyPair == currencyPair, cancellationToken);
    }

    public void Add(ExchangeRate exchangeRate)
    {
        ArgumentNullException.ThrowIfNull(exchangeRate);

        dbContext.ExchangeRates.Add(exchangeRate);
    }
}
