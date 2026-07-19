namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

public class GainResponse
{
	public DateOnly Date { get; set; }

	public List<GainItem> Items { get; init; } = [];

	public List<ExchangeRateInfo> ConversionRates { get; set; }

	public decimal TotalGain { get; set; }
}