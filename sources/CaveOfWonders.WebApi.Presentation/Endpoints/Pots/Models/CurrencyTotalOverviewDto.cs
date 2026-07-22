using DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Overview of totals for a specific currency
/// </summary>
public class CurrencyTotalOverviewDto
{
	/// <summary>
	/// Total value in the original currency
	/// </summary>
	public CurrencyValueDto Value { get; set; }

	/// <summary>
	/// Total value normalized to a common currency
	/// </summary>
	public CurrencyValueDto NormalizedValue { get; set; }

	/// <summary>
	/// Percentage of this currency relative to the total portfolio
	/// </summary>
	public decimal Percentage { get; set; }

	internal static CurrencyTotalOverviewDto From(CurrencyTotalOverview currencyTotalOverview)
	{
		if (currencyTotalOverview == null)
			return null;

		return new CurrencyTotalOverviewDto
		{
			Value = CurrencyValueDto.From(currencyTotalOverview.Value),
			NormalizedValue = CurrencyValueDto.From(currencyTotalOverview.NormalizedValue),
			Percentage = currencyTotalOverview.Percentage
		};
	}
}