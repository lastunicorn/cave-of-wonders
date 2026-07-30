namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

public class DailyExchangeRates
{
	public DateOnly Date { get; set; }

	public List<ExchangeRateForCurrency> ExchangeRates { get; set; }
}