namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

internal class PotsAnalysis
{
	private readonly CurrenciesConvertor currenciesConvertor;

	public List<PotInstanceInfo> PotInstanceInfos { get; set; }

	public string TargetCurrency { get; set; }

	public DateOnly TargetDate { get; set; }

	public decimal TotalValue { get; private set; }

	public List<CurrencyTotalOverview> currencyTotalOverviews;

	public PotsAnalysis(CurrenciesConvertor currenciesConvertor)
	{
		this.currenciesConvertor = currenciesConvertor ?? throw new ArgumentNullException(nameof(currenciesConvertor));
	}

	public async Task Calculate(CancellationToken cancellationToken = default)
	{
		TotalValue = PotInstanceInfos.Sum(x => x.NormalizedValue?.Value ?? 0);
		currencyTotalOverviews = await CalculateCurrencyTotalOverviews(cancellationToken);
	}

	private async Task<List<CurrencyTotalOverview>> CalculateCurrencyTotalOverviews(CancellationToken cancellationToken)
	{
		// Group pot instances by currency and calculate the sum for each currency
		IEnumerable<IGrouping<string, PotInstanceInfo>> currencyGroups = PotInstanceInfos
			.Where(x => x.Value != null)
			.GroupBy(x => x.Value.Currency);

		List<CurrencyTotalOverview> overviews = [];

		foreach (IGrouping<string, PotInstanceInfo> group in currencyGroups)
		{
			string currency = group.Key;
			decimal value = group.Sum(x => x.Value?.Value ?? 0);

			DatedAmount datedAmount = new()
			{
				Currency = currency,
				Value = value,
				Date = TargetDate
			};

			DatedAmount normalizedValue = await CalculateNormalizedValue(datedAmount, cancellationToken);

			// Calculate percentage
			decimal percentage = TotalValue > 0
				? (normalizedValue.Value / TotalValue) * 100
				: 0;

			// Create and add the overview
			CurrencyTotalOverview overview = new()
			{
				Value = datedAmount,
				NormalizedValue = normalizedValue,
				Percentage = percentage
			};

			overviews.Add(overview);
		}

		return overviews;
	}

	private async Task<DatedAmount> CalculateNormalizedValue(DatedAmount datedAmount, CancellationToken cancellationToken)
	{
		return datedAmount.Currency == TargetCurrency
			? datedAmount
			: await currenciesConvertor.Convert(datedAmount, TargetCurrency, TargetDate, cancellationToken);
	}
}