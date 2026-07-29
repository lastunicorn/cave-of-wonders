using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using LiteDB;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class ExchangeRateRepository : IExchangeRateRepository
{
	private readonly DbContext dbContext;

	public ExchangeRateRepository(DbContext dbContext)
	{
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public Task<IEnumerable<ExchangeRate>> Get(CurrencyPair[] currencyPairs, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ILiteQueryable<ExchangeRateDbEntity> query = dbContext.ExchangeRates.Query();

		if (currencyPairs != null && currencyPairs.Length > 0)
		{
			List<string> currencyPairsAsStrings = currencyPairs
				.Select(x => x.ToString())
				.ToList();

			query = query.Where(x => currencyPairsAsStrings.Contains(x.CurrencyPair));
		}

		IEnumerable<ExchangeRate> exchangeRates = query
			.OrderBy(x => x.Date)
			.ToEnumerable()
			.Select(dbContext.ExchangeRateTracker.GetOrAttach);

		return Task.FromResult(exchangeRates);
	}

	public Task<ExchangeRate> GetForLatestDayAvailable(CurrencyPair currencyPair, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		string currencyPairAsString = currencyPair.ToString();
		string invertedCurrencyPairAsString = currencyPair.Invert().ToString();

		ExchangeRateDbEntity exchangeRateDbEntity = dbContext.ExchangeRates.Query()
			.Where(x => x.Date <= date && (x.CurrencyPair == currencyPairAsString || (allowInverted && x.CurrencyPair == invertedCurrencyPairAsString)))
			.OrderByDescending(x => x.Date)
			.FirstOrDefault();

		if (exchangeRateDbEntity == null)
			return Task.FromResult<ExchangeRate>(null);

		ExchangeRate exchangeRate = dbContext.ExchangeRateTracker.GetOrAttach(exchangeRateDbEntity);

		return Task.FromResult(exchangeRate);
	}

	public Task<IEnumerable<ExchangeRate>> GetForLatestDayAvailable(CurrencyPair[] currencyPairs, DateOnly date, bool allowInverted = false, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		List<string> currencyPairsAsStrings = currencyPairs
			.Select(x => x.ToString())
			.ToList();

		IEnumerable<ExchangeRate> exchangeRates = dbContext.ExchangeRates.Query()
			.Where(x => currencyPairsAsStrings.Contains(x.CurrencyPair) && x.Date <= date)
			.ToList()
			.GroupBy(x => x.Date)
			.OrderByDescending(x => x.Key)
			.FirstOrDefault()
			.Select(dbContext.ExchangeRateTracker.GetOrAttach);

		return Task.FromResult(exchangeRates);
	}

	public Task<IEnumerable<ExchangeRate>> GetByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ILiteQueryable<ExchangeRateDbEntity> query = dbContext.ExchangeRates.Query();

		if (currencyPairs != null && currencyPairs.Length > 0)
		{
			List<string> currencyPairsAsStrings = currencyPairs
				.Select(x => x.ToString())
				.ToList();

			query = query.Where(x => currencyPairsAsStrings.Contains(x.CurrencyPair));
		}

		if (startDate != null)
			query = query.Where(x => x.Date >= startDate.Value);

		if (endDate != null)
			query = query.Where(x => x.Date <= endDate.Value);

		query = query.OrderBy(x => x.Date);

		IEnumerable<ExchangeRate> exchangeRates = query
			.ToEnumerable()
			.Select(dbContext.ExchangeRateTracker.GetOrAttach);

		return Task.FromResult(exchangeRates);
	}

	public Task<IEnumerable<ExchangeRate>> GetByYear(CurrencyPair[] currencyPairs, uint year, uint? month, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ILiteQueryable<ExchangeRateDbEntity> query = dbContext.ExchangeRates.Query();

		if (currencyPairs != null && currencyPairs.Length > 0)
		{
			List<string> currencyPairsAsStrings = currencyPairs
				.Select(x => x.ToString())
				.ToList();

			query = query.Where(x => currencyPairsAsStrings.Contains(x.CurrencyPair));
		}

		query = query.Where(x => x.Date.Year == year);

		if (month != null)
			query = query.Where(x => x.Date.Month == month.Value);

		query = query.OrderBy(x => x.Date);

		IEnumerable<ExchangeRate> exchangeRates = query
			.ToEnumerable()
			.Select(dbContext.ExchangeRateTracker.GetOrAttach);

		return Task.FromResult(exchangeRates);
	}

	public Task<ExchangeRate> Get(CurrencyPair currencyPair, DateOnly date, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		string currencyPairAsString = currencyPair.ToString();

		ExchangeRateDbEntity entity = dbContext.ExchangeRates
			.FindOne(x => x.Date == date && x.CurrencyPair == currencyPairAsString);

		if (entity == null)
			return Task.FromResult<ExchangeRate>(null);

		ExchangeRate exchangeRate = dbContext.ExchangeRateTracker.GetOrAttach(entity);

		return Task.FromResult(exchangeRate);
	}

	public void Add(ExchangeRate exchangeRate)
	{
		dbContext.ExchangeRateTracker.TrackNew(exchangeRate);
	}
}