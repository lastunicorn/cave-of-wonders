using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gain;

internal class GainViewModel
{
	public List<GainItem> Items { get; }

	public List<ExchangeRateViewModel> ConversionRates { get; }

	public DatedAmount TotalGain { get; }

	internal GainViewModel(GainResponse response)
	{
		Items = response.Items;

		ConversionRates = response.ConversionRates
			.Select(x => new ExchangeRateViewModel(x, x.Date == response.Date))
			.ToList();

		TotalGain = response.TotalGain;
	}
}