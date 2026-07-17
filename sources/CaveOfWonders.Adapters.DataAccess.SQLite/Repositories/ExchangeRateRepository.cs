using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
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
        IQueryable<ExchangeRateEntity> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
        {
            string[] pairsAsStrings = currencyPairs.Select(cp => cp.ToString()).ToArray();
            query = query.Where(x => pairsAsStrings.Contains(x.CurrencyPair));
        }

        List<ExchangeRateEntity> entities = await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);

        return entities.Select(dbContext.ExchangeRateTracker.GetOrAttach);
    }

    public async Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
    {
        string pairAsString = currencyPair.ToString();

        List<string> pairsAsStrings = allowInverted
            ? [pairAsString, currencyPair.Invert().ToString()]
            : [pairAsString];

        ExchangeRateEntity entity = await dbContext.ExchangeRates
            .Where(x => pairsAsStrings.Contains(x.CurrencyPair) && x.Date <= date)
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);

        return entity == null ? null : dbContext.ExchangeRateTracker.GetOrAttach(entity);
    }

    public async Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
    {
        string[] pairsAsStrings = currencyPairs.Select(cp => cp.ToString()).ToArray();

        DateOnly? maxDate = await dbContext.ExchangeRates
            .Where(x => pairsAsStrings.Contains(x.CurrencyPair) && x.Date <= date)
            .MaxAsync(x => (DateOnly?)x.Date, cancellationToken);

        if (maxDate == null)
            return Enumerable.Empty<ExchangeRate>();

        List<ExchangeRateEntity> entities = await dbContext.ExchangeRates
            .Where(x => pairsAsStrings.Contains(x.CurrencyPair) && x.Date == maxDate)
            .ToListAsync(cancellationToken);

        return entities.Select(dbContext.ExchangeRateTracker.GetOrAttach);
    }

    public async Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default)
    {
        IQueryable<ExchangeRateEntity> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
        {
            string[] pairsAsStrings = currencyPairs.Select(cp => cp.ToString()).ToArray();
            query = query.Where(x => pairsAsStrings.Contains(x.CurrencyPair));
        }

        if (startDate != null)
            query = query.Where(x => x.Date >= startDate.Value);

        if (endDate != null)
            query = query.Where(x => x.Date <= endDate.Value);

        List<ExchangeRateEntity> entities = await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);

        return entities.Select(dbContext.ExchangeRateTracker.GetOrAttach);
    }

    public async Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month, CancellationToken cancellationToken = default)
    {
        IQueryable<ExchangeRateEntity> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
        {
            string[] pairsAsStrings = currencyPairs.Select(cp => cp.ToString()).ToArray();
            query = query.Where(x => pairsAsStrings.Contains(x.CurrencyPair));
        }

        query = query.Where(x => x.Date.Year == (int)year);

        if (month != null)
            query = query.Where(x => x.Date.Month == (int)month.Value);

        List<ExchangeRateEntity> entities = await query.OrderBy(x => x.Date).ToListAsync(cancellationToken);

        return entities.Select(dbContext.ExchangeRateTracker.GetOrAttach);
    }

    public async Task<ExchangeRate> Get(CurrencyPair currencyPair, DateOnly date, CancellationToken cancellationToken = default)
    {
        string pairAsString = currencyPair.ToString();

        ExchangeRateEntity entity = await dbContext.ExchangeRates
            .FirstOrDefaultAsync(x => x.Date == date && x.CurrencyPair == pairAsString, cancellationToken);

        return entity == null ? null : dbContext.ExchangeRateTracker.GetOrAttach(entity);
    }

    public void Add(ExchangeRate exchangeRate)
    {
        dbContext.ExchangeRateTracker.TrackNew(exchangeRate);
    }
}
