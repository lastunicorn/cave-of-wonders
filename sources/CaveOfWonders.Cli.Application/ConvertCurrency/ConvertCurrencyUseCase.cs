using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ConvertCurrency;

internal class ConvertCurrencyUseCase : IRequestHandler<ConvertCurrencyRequest, ConvertCurrencyResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public ConvertCurrencyUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<ConvertCurrencyResponse> Handle(ConvertCurrencyRequest request, CancellationToken cancellationToken)
	{
		DateOnly dateOfExchangeRate = request.Date ?? systemClock.Today;
		ExchangeRate exchangeRate = await RetrieveExchangeRate(request.CurrencyPair, dateOfExchangeRate, cancellationToken);

		return new ConvertCurrencyResponse
		{
			InitialValue = request.InitialValue,
			ConvertedValue = exchangeRate.Convert(request.InitialValue),
			ExchangeRate = new ExchangeRateInfo(exchangeRate),
			IsDateCurrent = exchangeRate.Date == dateOfExchangeRate
		};
	}

	private async Task<ExchangeRate> RetrieveExchangeRate(CurrencyPair currencyPair, DateOnly date, CancellationToken cancellationToken)
	{
		ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository.GetForLatestDayAvailable(currencyPair, date, true, cancellationToken);

		if (exchangeRate == null)
			throw new ExchangeRateNotFoundException(currencyPair, date);

		ConversionAbility conversionAbility = exchangeRate.AnalyzeConversionAbility(currencyPair.Currency1, currencyPair.Currency2);

		return conversionAbility switch
		{
			ConversionAbility.None => throw new ExchangeRateUnusableException(exchangeRate),
			ConversionAbility.ConvertDirect => exchangeRate,
			ConversionAbility.ConvertReverse => exchangeRate.Invert(),
			_ => throw new ArgumentOutOfRangeException(),
		};
	}
}