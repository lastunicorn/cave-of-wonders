using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal class PotsAnalysis
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ILog log;
	private readonly CurrencyConverter currencyConverter;

	public List<Pot> Pots { get; set; }

	public Currency TargetCurrency { get; set; }

	public DateOnly TargetDate { get; set; }

	public SnapshotSelectionMode SnapshotSelectionMode { get; set; } = SnapshotSelectionMode.LastAvailable;

	public decimal TotalValue { get; private set; }

	public List<CurrencyOverview> CurrencyOverviews { get; } = [];

	public List<PotInstanceInfo> PotInstanceInfos { get; } = [];

	public PotsAnalysis(IUnitOfWork unitOfWork, ILog log, CurrencyConverter currencyConverter)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.log = log ?? throw new ArgumentNullException(nameof(log));
		this.currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		foreach (Pot pot in Pots)
		{
			PotInstanceInfo potInstanceInfo = await CreatePotInstanceInfo(pot, cancellationToken);
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

	private async Task<PotInstanceInfo> CreatePotInstanceInfo(Pot pot, CancellationToken cancellationToken)
	{
		PotAnalysis potAnalysis = new(unitOfWork, log, currencyConverter)
		{
			Pot = pot,
			TargetDate = TargetDate,
			TargetCurrency = TargetCurrency,
			SnapshotSelectionMode = SnapshotSelectionMode
		};

		await potAnalysis.ExecuteAsync(cancellationToken);

		return new PotInstanceInfo
		{
			Id = pot.Id,
			Name = pot.Name,
			IsActive = pot.IsActive(TargetDate),
			Value = potAnalysis.Value,
			NormalizedValue = potAnalysis.NormalizedValue
		};
	}
}