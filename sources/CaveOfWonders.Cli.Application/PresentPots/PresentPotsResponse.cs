namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class PresentPotsResponse
{
	public DateOnly Date { get; set; }

	public List<PotInstanceInfo> PotInstances { get; set; }

	public List<ExchangeRateInfo> ConversionRates { get; set; }

	public DatedAmount Total { get; set; }

	public List<CurrencyTotalOverview> CurrencyTotalOverviews { get; set; } = [];
}