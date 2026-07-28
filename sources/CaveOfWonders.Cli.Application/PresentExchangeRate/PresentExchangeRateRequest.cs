namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

public class PresentExchangeRateRequest
{
	public string CurrencyPair { get; set; }

	public bool Today { get; set; }

	public DateOnly? Date { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public uint? Year { get; set; }

	public uint? Month { get; set; }
}