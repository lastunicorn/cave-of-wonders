using DustInTheWind.CaveOfWonders.Cli.Application;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Represents a monetary value with its currency and date
/// </summary>
public class CurrencyValueDto
{
	/// <summary>
	/// The monetary value
	/// </summary>
	public decimal Value { get; set; }

	/// <summary>
	/// The currency code (e.g., USD, EUR, RON)
	/// </summary>
	public string Currency { get; set; }

	/// <summary>
	/// The date when this value was recorded or is valid
	/// </summary>
	public DateOnly Date { get; set; }

	internal static CurrencyValueDto From(DatedAmount datedAmount)
	{
		if (datedAmount == null)
			return null;

		return new CurrencyValueDto
		{
			Value = datedAmount.Value,
			Currency = datedAmount.Currency,
			Date = datedAmount.Date
		};
	}
}