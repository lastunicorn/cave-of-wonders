using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class PresentPotsUseCase : IRequestHandler<PresentPotsRequest, PresentPotsResponse>
{
	private readonly ISystemClock systemClock;
	private readonly IUnitOfWork unitOfWork;
	private readonly CurrenciesConvertor currenciesConverter;

	public PresentPotsUseCase(ISystemClock systemClock, IUnitOfWork unitOfWork)
	{
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

		currenciesConverter = new CurrenciesConvertor(unitOfWork);
	}

	public async Task<PresentPotsResponse> Handle(PresentPotsRequest request, CancellationToken cancellationToken)
	{
		DateOnly currentDate = request.Date ?? systemClock.Today;
		string defaultCurrency = request.Currency ?? "RON";

		IEnumerable<PotSnapshot> potSnapshots = await RetrievePotSnapshotsFromStorage(currentDate, request.IncludeInactive, cancellationToken);
		List<PotInstanceInfo> potInstanceInfos = await ConvertToPotInstanceInfos(potSnapshots, currentDate, defaultCurrency, cancellationToken);

		PotsAnalysis potsAnalysis = new(currenciesConverter)
		{
			PotInstanceInfos = potInstanceInfos,
			TargetDate = currentDate,
			TargetCurrency = defaultCurrency
		};

		await potsAnalysis.Calculate(cancellationToken);

		return new PresentPotsResponse
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

	private async Task<IEnumerable<PotSnapshot>> RetrievePotSnapshotsFromStorage(DateOnly date, bool includeInactive, CancellationToken cancellationToken)
	{
		IEnumerable<PotSnapshot> potSnapshots = await unitOfWork.PotRepository.GetSnapshotsAsync(date, DateMatchingMode.LastAvailable, includeInactive, cancellationToken);
		return potSnapshots.OrderBy(x => x.Pot.DisplayOrder);
	}

	private async Task<List<PotInstanceInfo>> ConvertToPotInstanceInfos(IEnumerable<PotSnapshot> potInstances, DateOnly currentDate, string defaultCurrency, CancellationToken cancellationToken)
	{
		List<PotInstanceInfo> potInstanceInfos = [];

		foreach (PotSnapshot potSnapshot in potInstances)
		{
			PotInstanceInfo potInstanceInfo = await Convert(potSnapshot, currentDate, defaultCurrency, cancellationToken);
			potInstanceInfos.Add(potInstanceInfo);
		}

		return potInstanceInfos;
	}

	private async Task<PotInstanceInfo> Convert(PotSnapshot potSnapshot, DateOnly currentDate, Currency defaultCurrency, CancellationToken cancellationToken)
	{
		PotInstanceInfo potInstanceInfo = new()
		{
			Id = potSnapshot.Pot.Id,
			Name = potSnapshot.Pot.Name,
			IsActive = potSnapshot.Pot.IsActive(currentDate),
			Value = ComputeOriginalValue(potSnapshot)
		};

		potInstanceInfo.NormalizedValue = await currenciesConverter.Convert(potInstanceInfo.Value, defaultCurrency, currentDate, cancellationToken);

		return potInstanceInfo;
	}

	private static DatedAmount ComputeOriginalValue(PotSnapshot potSnapshot)
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

		return null;
	}
}