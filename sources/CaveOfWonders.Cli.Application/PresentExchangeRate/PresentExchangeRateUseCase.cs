using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

internal class PresentExchangeRateUseCase : IUseCase<PresentExchangeRateRequest, PresentExchangeRateResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;
	private PresentExchangeRateResponse response;

	public PresentExchangeRateUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<PresentExchangeRateResponse> Execute(PresentExchangeRateRequest request, CancellationToken cancellationToken)
	{
		response = new PresentExchangeRateResponse();

		/*
		 * If "today" or specific date is requested, the exchange rate for today is displayed.
		 *  - return requested currency pairs for that day.
		 */

		CurrencyPair[] currencyPairs = request.CurrencyPair?.ParseCurrencyPairs().ToArray() ?? [];

		if (request.Today)
			await RetrieveForToday(currencyPairs, cancellationToken);
		else if (request.Date != null)
			await RetrieveByDate(currencyPairs, request.Date.Value, cancellationToken);
		else if (request.Year != null)
			await RetrieveByYear(currencyPairs, request.Year.Value, request.Month, cancellationToken);
		else if (request.StartDate != null || request.EndDate != null)
			await RetrieveByDateInterval(currencyPairs, request.StartDate, request.EndDate, cancellationToken);
		else
			await RetrieveAll(currencyPairs, cancellationToken);

		return response;
	}

	private Task RetrieveForToday(CurrencyPair[] currencyPairs, CancellationToken cancellationToken)
	{
		// If currency pair provided        => 1 currency; multiple dates       => display by currency
		// If currency pair NOT provided    => multiple currencies; multiple dates       => display by currency

		DateOnly dateTime = systemClock.Today;
		return RetrieveByDate(currencyPairs, dateTime, cancellationToken);
	}

	private async Task RetrieveByDate(CurrencyPair[] currencyPairs, DateOnly date, CancellationToken cancellationToken)
	{
		IEnumerable<ExchangeRate> exchangeRates = await unitOfWork.ExchangeRateRepository.GetForLatestDayAvailable(currencyPairs, date, cancellationToken: cancellationToken);

		if (exchangeRates == null)
			throw new ExchangeRateNotFoundException(currencyPairs, date);

		DateOnly? actualDate = exchangeRates
			.FirstOrDefault()?.Date;

		DailyExchangeRates dailyExchangeRates = new()
		{
			Date = actualDate ?? date,
			ExchangeRates = exchangeRates
				.Select(x => new ExchangeRateForCurrency
				{
					CurrencyPair = x.CurrencyPair,
					Value = x.Value
				})
				.ToList()
		};

		response.DailyExchangeRates = [dailyExchangeRates];

		if (dailyExchangeRates.ExchangeRates.Count > 0 && actualDate != date)
		{
			response.Comments = new ExchangeRatesNotFoundNote
			{
				CurrencyPairs = currencyPairs?.ToList(),
				Date = date
			};
		}
	}

	private async Task RetrieveByYear(CurrencyPair[] currencyPairs, uint year, uint? month, CancellationToken cancellationToken)
	{
		response.DailyExchangeRates = (await unitOfWork.ExchangeRateRepository.GetByYear(currencyPairs, year, month, cancellationToken))
			.GroupBy(x => x.Date)
			.Select(x => new DailyExchangeRates
			{
				Date = x.Key,
				ExchangeRates = x
					.Select(y => new ExchangeRateForCurrency
					{
						CurrencyPair = y.CurrencyPair,
						Value = y.Value
					})
					.ToList()
			})
			.OrderBy(x => x.Date)
			.ToList();
	}

	private async Task RetrieveByDateInterval(CurrencyPair[] currencyPairs, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
	{
		response.DailyExchangeRates = (await unitOfWork.ExchangeRateRepository.GetByDateInterval(currencyPairs, startDate, endDate, cancellationToken))
			.GroupBy(x => x.Date)
			.Select(x => new DailyExchangeRates
			{
				Date = x.Key,
				ExchangeRates = x
					.Select(y => new ExchangeRateForCurrency
					{
						CurrencyPair = y.CurrencyPair,
						Value = y.Value
					})
					.ToList()
			})
			.OrderBy(x => x.Date)
			.ToList();
	}

	private async Task RetrieveAll(CurrencyPair[] currencyPairs, CancellationToken cancellationToken)
	{
		response.DailyExchangeRates = (await unitOfWork.ExchangeRateRepository.Get(currencyPairs, cancellationToken))
			.GroupBy(x => x.Date)
			.Select(x => new DailyExchangeRates
			{
				Date = x.Key,
				ExchangeRates = x
					.Select(y => new ExchangeRateForCurrency
					{
						CurrencyPair = y.CurrencyPair,
						Value = y.Value
					})
					.ToList()
			})
			.OrderBy(x => x.Date)
			.ToList();
	}
}