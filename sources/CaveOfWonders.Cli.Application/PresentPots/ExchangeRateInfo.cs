using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class ExchangeRateInfo
{
	public string SourceCurrency { get; }

	public string DestinationCurrency { get; }

	public DateOnly Date { get; }

	public decimal Value { get; }

	internal ExchangeRateInfo(ExchangeRate conversionRate)
	{
		SourceCurrency = conversionRate.CurrencyPair.Currency1;
		DestinationCurrency = conversionRate.CurrencyPair.Currency2;
		Date = conversionRate.Date;
		Value = conversionRate.Value;
	}
}