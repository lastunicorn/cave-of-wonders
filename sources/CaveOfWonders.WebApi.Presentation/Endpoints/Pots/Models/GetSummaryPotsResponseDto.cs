using DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Response containing all financial pots with their values and metadata
/// </summary>
public class GetSummaryPotsResponseDto
{
	/// <summary>
	/// Date for which the pot values are calculated
	/// </summary>
	public DateOnly Date { get; set; }

	/// <summary>
	/// List of pot instances with their current values
	/// </summary>
	public List<PotInstanceInfoDto> PotInstances { get; set; }

	/// <summary>
	/// Exchange rates used for currency conversion
	/// </summary>
	public List<ExchangeRateInfoDto> ConversionRates { get; set; }

	/// <summary>
	/// Total value of all pots combined
	/// </summary>
	public CurrencyValueDto Total { get; set; }

	/// <summary>
	/// Overview of totals grouped by currency
	/// </summary>
	public List<CurrencyTotalOverviewDto> CurrencyTotalOverviews { get; set; }

	internal static GetSummaryPotsResponseDto From(PresentWealthResponse response)
	{
		return new GetSummaryPotsResponseDto
		{
			Date = response.Date,
			PotInstances = response.PotInstances?
					.Select(PotInstanceInfoDto.From)
					.ToList()
				?? [],
			ConversionRates = response.ConversionRates?
					.Select(ExchangeRateInfoDto.From)
					.ToList()
				?? [],
			Total = CurrencyValueDto.From(response.Total),
			CurrencyTotalOverviews = response.CurrencyOverviews?
					.Select(CurrencyTotalOverviewDto.From)
					.ToList()
				?? []
		};
	}
}