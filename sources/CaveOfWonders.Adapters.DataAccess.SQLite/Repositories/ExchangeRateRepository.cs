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

    public async Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs)
    {
        IQueryable<ExchangeRateEntity> query = dbContext.ExchangeRates;

        if (currencyPairs != null && currencyPairs.Length > 0)
        {
            string[] pairsAsStrings = currencyPairs.Select(cp => cp.ToString()).ToArray();
            query = query.Where(x => pairsAsStrings.Contains(x.CurrencyPair));
        }

        List<ExchangeRateEntity> entities = await query.OrderBy(x => x.Date).ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false)
    {
        string pairAsString = currencyPair.ToString();

        List<string> pairsAsStrings = allowInverted
            ? [pairAsString, currencyPair.Invert().ToString()]
            : [pairAsString];

        ExchangeRateEntity entity = await dbContext.ExchangeRates
            .Where(x => pairsAsStrings.Contains(x.CurrencyPair) && x.Date <= date)
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync();

        return entity == null ? null : MapToDomain(entity);
    }

    public async Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false)
    {
        string[] pairsAsStrings = currencyPairs.Select(cp => cp.ToString()).ToArray();

        DateOnly? maxDate = await dbContext.ExchangeRates
            .Where(x => pairsAsStrings.Contains(x.CurrencyPair) && x.Date <= date)
            .MaxAsync(x => (DateOnly?)x.Date);

        if (maxDate == null)
            return Enumerable.Empty<ExchangeRate>();

        List<ExchangeRateEntity> entities = await dbContext.ExchangeRates
            .Where(x => pairsAsStrings.Contains(x.CurrencyPair) && x.Date == maxDate)
            .ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate)
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

        List<ExchangeRateEntity> entities = await query.OrderBy(x => x.Date).ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month)
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

        List<ExchangeRateEntity> entities = await query.OrderBy(x => x.Date).ToListAsync();

        return entities.Select(MapToDomain);
    }

    public async Task<ExchangeRateImportReport> Import(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        ExchangeRateImportReport report = new();
        Dictionary<(DateOnly, string), ExchangeRateEntity> pendingAdds = new();

        foreach (ExchangeRate exchangeRate in exchangeRates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string pairAsString = exchangeRate.CurrencyPair;
            var key = (exchangeRate.Date, pairAsString);

            ExchangeRateEntity existing = await dbContext.ExchangeRates
                .FirstOrDefaultAsync(x => x.Date == exchangeRate.Date && x.CurrencyPair == pairAsString, cancellationToken);

            if (existing != null)
            {
                if (existing.Value == exchangeRate.Value)
                {
                    report.ExistingIdenticalCount++;
                }
                else
                {
                    report.Updates.Add(new UpdateReport
                    {
                        Date = existing.Date,
                        CurrencyPair = existing.CurrencyPair,
                        OldValue = existing.Value,
                        NewValue = exchangeRate.Value
                    });
                    existing.Value = exchangeRate.Value;
                    report.ExistingUpdatedCount++;
                }
            }
            else if (pendingAdds.TryGetValue(key, out ExchangeRateEntity pending))
            {
                if (pending.Value == exchangeRate.Value)
                {
                    report.NewDuplicateIdenticalCount++;
                }
                else
                {
                    report.Duplicates.Add(new DuplicateReport
                    {
                        Date = pending.Date,
                        CurrencyPair = pending.CurrencyPair,
                        Value1 = pending.Value,
                        Value2 = exchangeRate.Value
                    });
                    pending.Value = exchangeRate.Value;
                    report.NewDuplicateDifferentCount++;
                }
            }
            else
            {
                ExchangeRateEntity newEntity = new()
                {
                    Date = exchangeRate.Date,
                    CurrencyPair = pairAsString,
                    Value = exchangeRate.Value
                };
                dbContext.ExchangeRates.Add(newEntity);
                pendingAdds[key] = newEntity;
                report.AddedCount++;
            }

            report.TotalCount++;
        }

        return report;
    }

    private static ExchangeRate MapToDomain(ExchangeRateEntity entity)
    {
        return new ExchangeRate
        {
            Date = entity.Date,
            CurrencyPair = entity.CurrencyPair,
            Value = entity.Value
        };
    }
}
