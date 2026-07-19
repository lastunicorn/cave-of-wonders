using DustInTheWind.CaveOfWonders.Cli.Application;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Exchange rate information for currency conversion
/// </summary>
public class ExchangeRateInfoDto
{
	/// <summary>
	/// Source currency
	/// </summary>
	public string SourceCurrency { get; set; }

	/// <summary>
	/// Destination currency
	/// </summary>
	public string DestinationCurrency { get; set; }

	/// <summary>
	/// Date of the exchange rate
	/// </summary>
	public DateOnly Date { get; set; }

	/// <summary>
	/// Exchange rate value
	/// </summary>
	public decimal Value { get; set; }

	internal static ExchangeRateInfoDto From(ExchangeRateInfo exchangeRateInfo)
	{
		return new ExchangeRateInfoDto
		{
			SourceCurrency = exchangeRateInfo.SourceCurrency,
			DestinationCurrency = exchangeRateInfo.DestinationCurrency,
			Date = exchangeRateInfo.Date,
			Value = exchangeRateInfo.Value
		};
	}
}