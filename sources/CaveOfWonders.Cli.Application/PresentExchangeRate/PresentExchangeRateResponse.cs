namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

public class PresentExchangeRateResponse
{
	public List<DailyExchangeRates> DailyExchangeRates { get; set; }

	public INote Comments { get; set; }
}