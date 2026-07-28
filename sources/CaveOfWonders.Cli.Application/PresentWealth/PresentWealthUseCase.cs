using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

public class PresentWealthUseCase : IUseCase<PresentWealthRequest, PresentWealthResponse>
{
	private readonly ISystemClock systemClock;
	private readonly IUnitOfWork unitOfWork;
	private readonly ILog log;
	private readonly CurrencyConverter currencyConverter;

	public PresentWealthUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork, ILog log)
	{
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.log = log ?? throw new ArgumentNullException(nameof(log));

		currencyConverter = new CurrencyConverter(unitOfWork);
	}

	public async Task<PresentWealthResponse> Execute(PresentWealthRequest request, CancellationToken cancellationToken)
	{
		DateOnly currentDate = request.Date ?? systemClock.Today;
		string defaultCurrency = request.Currency ?? "EUR";

		List<Pot> pots = await RetrievePots(request.IncludeInactive, currentDate, cancellationToken);

		PotsAnalysis potsAnalysis = new(unitOfWork, log, currencyConverter)
		{
			Pots = pots,
			TargetDate = currentDate,
			TargetCurrency = defaultCurrency
		};

		await potsAnalysis.ExecuteAsync(cancellationToken);

		return new PresentWealthResponse
		{
			Date = currentDate,
			PotInstances = potsAnalysis.PotInstanceInfos,
			ConversionRates = currencyConverter.UsedExchangeRates
				.Select(x => new ExchangeRateInfo(x))
				.ToList(),
			Total = new DatedAmount
			{
				Value = potsAnalysis.TotalValue,
				Currency = defaultCurrency
			},
			CurrencyOverviews = potsAnalysis.CurrencyOverviews
		};
	}

	private async Task<List<Pot>> RetrievePots(bool includeInactive, DateOnly date, CancellationToken cancellationToken)
	{
		IAsyncEnumerable<Pot> pots = unitOfWork.PotRepository.GetAllAsync(cancellationToken);

		if (!includeInactive)
			pots = pots.Where(x => x.IsActive(date));

		return await pots
			.OrderBy(x => x.DisplayOrder)
			.ToListAsync(cancellationToken);
	}
}