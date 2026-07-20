using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application;

internal class CurrenciesConvertor
{
	private readonly IUnitOfWork unitOfWork;

	public List<ExchangeRate> UsedExchangeRates { get; } = [];

	public CurrenciesConvertor(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<DatedAmount> Convert(DatedAmount originalValue, Currency destinationCurrency, DateOnly destinationDate, CancellationToken cancellationToken = default)
	{
		if (originalValue == null)
			return null;

		if (originalValue.Currency != destinationCurrency)
		{
			CurrencyConvertor currencyConverter = await GetConverter(originalValue.Currency, destinationCurrency, destinationDate, cancellationToken);

			return new DatedAmount
			{
				Currency = destinationCurrency,
				Value = currencyConverter.Convert(originalValue.Value),
				Date = currencyConverter.Date
			};
		}

		if (originalValue.Value == 0)
		{
			return new DatedAmount
			{
				Currency = destinationCurrency,
				Value = 0,
				Date = destinationDate
			};
		}

		return originalValue;
	}

	private async Task<CurrencyConvertor> GetConverter(Currency sourceCurrency, Currency destinationCurrency, DateOnly date, CancellationToken cancellationToken)
	{
		CurrencyPair currencyPair = new()
		{
			Currency1 = sourceCurrency,
			Currency2 = destinationCurrency
		};

		ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository
			.GetForLatestDayAvailable(currencyPair, date, true, cancellationToken);

		if (exchangeRate == null)
			return new CurrencyConvertor(date);

		if (!UsedExchangeRates.Contains(exchangeRate))
			UsedExchangeRates.Add(exchangeRate);

		bool isDirect = sourceCurrency == exchangeRate?.CurrencyPair.Currency1;

		return new CurrencyConvertor(exchangeRate, isDirect);
	}
}