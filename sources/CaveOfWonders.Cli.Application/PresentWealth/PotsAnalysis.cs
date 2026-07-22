using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal class PotsAnalysis
{
	private readonly CurrenciesConvertor currenciesConvertor;

	public List<PotInstanceInfo> PotInstanceInfos { get; set; }

	public string TargetCurrency { get; set; }

	public DateOnly TargetDate { get; set; }

	public decimal TotalValue { get; private set; }

	public List<CurrencyOverview> CurrencyOverviews { get; } = [];

	public PotsAnalysis(CurrenciesConvertor currenciesConvertor)
	{
		this.currenciesConvertor = currenciesConvertor ?? throw new ArgumentNullException(nameof(currenciesConvertor));
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		foreach (PotInstanceInfo potInstanceInfo in PotInstanceInfos)
		{
			TotalValue += potInstanceInfo.NormalizedValue?.Value ?? 0;

			CurrencyOverview currencyOverview = GetOrCreate(potInstanceInfo.Value.Currency);
			currencyOverview.Value += potInstanceInfo.Value;
		}

		foreach (CurrencyOverview currencyOverview in CurrencyOverviews)
		{
			currencyOverview.NormalizedValue = await CalculateNormalizedValue(currencyOverview.Value, cancellationToken);
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

	private async Task<DatedAmount> CalculateNormalizedValue(DatedAmount datedAmount, CancellationToken cancellationToken)
	{
		return datedAmount.Currency == TargetCurrency
			? datedAmount
			: await currenciesConvertor.Convert(datedAmount, TargetCurrency, TargetDate, cancellationToken);
	}
}