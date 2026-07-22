using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

public class PresentWealthUseCase : IRequestHandler<PresentWealthRequest, PresentWealthResponse>
{
	private readonly ISystemClock systemClock;
	private readonly IUnitOfWork unitOfWork;
	private readonly CurrencyConverter currencyConverter;

	public PresentWealthUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork)
	{
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

		currencyConverter = new CurrencyConverter(unitOfWork);
	}

	public async Task<PresentWealthResponse> Handle(PresentWealthRequest request, CancellationToken cancellationToken)
	{
		DateOnly currentDate = request.Date ?? systemClock.Today;
		string defaultCurrency = request.Currency ?? "EUR";

		List<Pot> pots = await RetrievePots(request.IncludeInactive, currentDate, cancellationToken);
		Dictionary<Guid, PotSnapshot> potSnapshotsByPotId = await RetrieveLatestSnapshotsFromStorage(currentDate, request.IncludeInactive, cancellationToken);

		PotsAnalysis potsAnalysis = new(currencyConverter)
		{
			Pots = pots,
			PotSnapshots = potSnapshotsByPotId,
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

	private async Task<Dictionary<Guid, PotSnapshot>> RetrieveLatestSnapshotsFromStorage(DateOnly date, bool includeInactive, CancellationToken cancellationToken)
	{
		IEnumerable<PotSnapshot> potSnapshots = await unitOfWork.PotSnapshotRepository.GetSnapshotsAsync(date, DateMatchingMode.LastAvailable, includeInactive, cancellationToken);
		return potSnapshots.ToDictionary(x => x.Pot.Id);
	}
}