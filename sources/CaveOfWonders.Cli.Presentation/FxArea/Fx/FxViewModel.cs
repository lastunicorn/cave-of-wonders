using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.Fx;

internal class FxViewModel
{
	public List<DailyExchangeRates> DailyExchangeRates { get; set; }

	public INote Comments { get; set; }
}