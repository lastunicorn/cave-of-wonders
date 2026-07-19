using DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Gain;

internal class GainViewModel
{
	public List<GainItem> Items { get; set; }

	public decimal TotalGain { get; set; }
}