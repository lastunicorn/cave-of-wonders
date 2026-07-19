using DustInTheWind.CaveOfWonders.Cli.Application;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

public class ExchangeRateViewModel
{
	public string SourceCurrency { get; }

	public string DestinationCurrency { get; }

	public decimal Value { get; }

	public DateOnly CurrencyDate { get; }

	public bool IsCurrent { get; }

	public ExchangeRateViewModel(ExchangeRateInfo exchangeRateInfo, bool isDateCurrent)
	{
		SourceCurrency = exchangeRateInfo.SourceCurrency;
		DestinationCurrency = exchangeRateInfo.DestinationCurrency;
		Value = exchangeRateInfo.Value;
		CurrencyDate = exchangeRateInfo.Date;
		IsCurrent = isDateCurrent;
	}

	public override string ToString()
	{
		return $"1 {SourceCurrency} = {Value} {DestinationCurrency} ({CurrencyDate:d})";
	}
}