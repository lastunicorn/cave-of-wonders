namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

public class PresentWealthResponse
{
	public DateOnly Date { get; set; }

	public List<PotInstanceInfo> PotInstances { get; set; }

	public List<ExchangeRateInfo> ConversionRates { get; set; }

	public DatedAmount Total { get; set; }

	public List<CurrencyTotalOverview> CurrencyTotalOverviews { get; set; } = [];
}