namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

public class CurrencyTotalOverview
{
	public DatedAmount Value { get; set; }

	public DatedAmount NormalizedValue { get; set; }

	public decimal Percentage { get; set; }
}