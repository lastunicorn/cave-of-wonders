using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal class PotsAnalysis
{
	private readonly CurrencyConverter currencyConverter;

	public List<Pot> Pots { get; set; }

	public Dictionary<Guid, PotSnapshot> PotSnapshots { get; set; }

	public string TargetCurrency { get; set; }

	public DateOnly TargetDate { get; set; }

	public decimal TotalValue { get; private set; }

	public List<CurrencyOverview> CurrencyOverviews { get; } = [];

	public List<PotInstanceInfo> PotInstanceInfos { get; } = [];

	public PotsAnalysis(CurrencyConverter currencyConverter)
	{
		this.currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		foreach (Pot pot in Pots)
		{
			bool snapshotExists = PotSnapshots.TryGetValue(pot.Id, out PotSnapshot potSnapshot);
			
			PotInstanceInfo potInstanceInfo = await CreatePotInstanceInfo(pot, potSnapshot, cancellationToken);
			PotInstanceInfos.Add(potInstanceInfo);
			
			TotalValue += potInstanceInfo.NormalizedValue?.Value ?? 0;

			CurrencyOverview currencyOverview = GetOrCreate(potInstanceInfo.Value.Currency);
			currencyOverview.Value += potInstanceInfo.Value;
			currencyOverview.NormalizedValue += potInstanceInfo.NormalizedValue;
		}

		foreach (CurrencyOverview currencyOverview in CurrencyOverviews)
		{
			currencyOverview.Percentage = TotalValue > 0
				? (currencyOverview.NormalizedValue.Value / TotalValue) * 100
				: 0;
		}
	}

	private CurrencyOverview GetOrCreate(Currency currency)
	{
		CurrencyOverview currencyOverview = CurrencyOverviews
			.FirstOrDefault(x => x.Value.Currency == currency);

		if (currencyOverview != null)
			return currencyOverview;

		currencyOverview = new CurrencyOverview
		{
			Value = new DatedAmount
			{
				Currency = currency,
				Date = TargetDate
			}
		};

		CurrencyOverviews.Add(currencyOverview);
		return currencyOverview;
	}

	private async Task<PotInstanceInfo> CreatePotInstanceInfo(Pot pot, PotSnapshot potSnapshot, CancellationToken cancellationToken)
	{
		DatedAmount value = ComputeSnapshotAmount(pot, potSnapshot, TargetDate);
		DatedAmount normalizedValue = await currencyConverter.Convert(value, TargetCurrency, TargetDate, cancellationToken);

		return new PotInstanceInfo
		{
			Id = pot.Id,
			Name = pot.Name,
			IsActive = pot.IsActive(TargetDate),
			Value = value,
			NormalizedValue = normalizedValue
		};
	}

	private static DatedAmount ComputeSnapshotAmount(Pot pot, PotSnapshot potSnapshot, DateOnly currentDate)
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

	private async Task<DatedAmount> CalculateNormalizedValue(DatedAmount datedAmount, CancellationToken cancellationToken)
	{
		return datedAmount.Currency == TargetCurrency
			? datedAmount
			: await currencyConverter.Convert(datedAmount, TargetCurrency, TargetDate, cancellationToken);
	}
}