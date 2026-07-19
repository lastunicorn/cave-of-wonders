namespace DustInTheWind.CaveOfWonders.Cli.Application.ConvertCurrency;

public class ConvertCurrencyResponse
{
	public decimal InitialValue { get; set; }

	public decimal ConvertedValue { get; set; }

	public bool IsDateCurrent { get; set; }

	public ExchangeRateInfo ExchangeRate { get; set; }
}