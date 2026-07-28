using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using System.Runtime.CompilerServices;

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
		await foreach (PotAnalysis potAnalysis in AnalyzePots(cancellationToken))
		{
			PotInstanceInfos.Add(new PotInstanceInfo
			{
				Id = potAnalysis.Pot.Id,
				Name = potAnalysis.Pot.Name,
				IsActive = potAnalysis.Pot.IsActive(TargetDate),
				Value = potAnalysis.Value,
				NormalizedValue = potAnalysis.NormalizedValue
			});

			TotalValue += potAnalysis.NormalizedValue?.Value ?? 0;

			CurrencyOverview currencyOverview = GetOrCreate(potAnalysis.Value.Currency);
			currencyOverview.Value += potAnalysis.Value;
			currencyOverview.NormalizedValue += potAnalysis.NormalizedValue;
		}

		foreach (CurrencyOverview currencyOverview in CurrencyOverviews)
		{
			currencyOverview.Percentage = TotalValue > 0
				? (currencyOverview.NormalizedValue.Value / TotalValue) * 100
				: 0;
		}
	}

	private async IAsyncEnumerable<PotAnalysis> AnalyzePots([EnumeratorCancellation] CancellationToken cancellationToken)
	{
		IEnumerable<PotAnalysis> potAnalyses = Pots
			.Select(x => new PotAnalysis(unitOfWork, log, currencyConverter)
			{
				Pot = x,
				TargetDate = TargetDate,
				TargetCurrency = TargetCurrency,
				SnapshotSelectionMode = SnapshotSelectionMode
			});

		foreach (PotAnalysis potAnalysis in potAnalyses)
		{
			await potAnalysis.ExecuteAsync(cancellationToken);
			yield return potAnalysis;
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
}