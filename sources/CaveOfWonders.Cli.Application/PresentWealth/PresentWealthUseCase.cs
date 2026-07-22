using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

public class PresentWealthUseCase : IRequestHandler<PresentWealthRequest, PresentWealthResponse>
{
	private readonly ISystemClock systemClock;
	private readonly IUnitOfWork unitOfWork;
	private readonly CurrenciesConvertor currenciesConverter;

	public PresentWealthUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork)
	{
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

		currenciesConverter = new CurrenciesConvertor(unitOfWork);
	}

	public async Task<PresentWealthResponse> Handle(PresentWealthRequest request, CancellationToken cancellationToken)
	{
		DateOnly currentDate = request.Date ?? systemClock.Today;
		string defaultCurrency = request.Currency ?? "EUR";

		List<Pot> pots = await RetrievePots(request.IncludeInactive, currentDate, cancellationToken);
		Dictionary<Guid, PotSnapshot> potSnapshotsByPotId = await RetrievePotSnapshotsFromStorage(currentDate, request.IncludeInactive, cancellationToken);
		List<PotInstanceInfo> potInstanceInfos = await ConvertToPotInstanceInfos(pots, potSnapshotsByPotId, currentDate, defaultCurrency, cancellationToken);

		PotsAnalysis potsAnalysis = new(currenciesConverter)
		{
			PotInstanceInfos = potInstanceInfos,
			TargetDate = currentDate,
			TargetCurrency = defaultCurrency
		};

		await potsAnalysis.Calculate(cancellationToken);

		return new PresentWealthResponse
		{
			Date = currentDate,
			PotInstances = potInstanceInfos,
			ConversionRates = currenciesConverter.UsedExchangeRates
				.Select(x => new ExchangeRateInfo(x))
				.ToList(),
			Total = new DatedAmount
			{
				Value = potsAnalysis.TotalValue,
				Currency = defaultCurrency
			},
			CurrencyTotalOverviews = potsAnalysis.currencyTotalOverviews
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

	private async Task<Dictionary<Guid, PotSnapshot>> RetrievePotSnapshotsFromStorage(DateOnly date, bool includeInactive, CancellationToken cancellationToken)
	{
		IEnumerable<PotSnapshot> potSnapshots = await unitOfWork.PotRepository.GetSnapshotsAsync(date, DateMatchingMode.LastAvailable, includeInactive, cancellationToken);
		return potSnapshots.ToDictionary(x => x.Pot.Id);
	}

	private async Task<List<PotInstanceInfo>> ConvertToPotInstanceInfos(List<Pot> pots, Dictionary<Guid, PotSnapshot> potSnapshotsByPotId, DateOnly currentDate, string defaultCurrency, CancellationToken cancellationToken)
	{
		List<PotInstanceInfo> potInstanceInfos = [];

		foreach (Pot pot in pots)
		{
			potSnapshotsByPotId.TryGetValue(pot.Id, out PotSnapshot potSnapshot);
			PotInstanceInfo potInstanceInfo = await Convert(pot, potSnapshot, currentDate, defaultCurrency, cancellationToken);
			potInstanceInfos.Add(potInstanceInfo);
		}

		return potInstanceInfos;
	}

	private async Task<PotInstanceInfo> Convert(Pot pot, PotSnapshot potSnapshot, DateOnly currentDate, Currency defaultCurrency, CancellationToken cancellationToken)
	{
		PotInstanceInfo potInstanceInfo = new()
		{
			Id = pot.Id,
			Name = pot.Name,
			IsActive = pot.IsActive(currentDate),
			Value = ComputeOriginalValue(pot, potSnapshot, currentDate)
		};

		potInstanceInfo.NormalizedValue = await currenciesConverter.Convert(potInstanceInfo.Value, defaultCurrency, currentDate, cancellationToken);

		return potInstanceInfo;
	}

	private static DatedAmount ComputeOriginalValue(Pot pot, PotSnapshot potSnapshot, DateOnly currentDate)
	{
		if (potSnapshot != null)
		{
			return new DatedAmount
			{
				Currency = potSnapshot.Pot.Currency,
				Value = potSnapshot.Value,
				Date = potSnapshot.Date
			};
		}

		return new DatedAmount
		{
			Currency = pot.Currency,
			Value = 0,
			Date = currentDate
		};
	}
}