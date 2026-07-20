using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gem;

internal class GemCommandViewModel
{
	public IReadOnlyList<GemDto> Gems { get; init; }

	public Amount TotalAmount { get; init; }
}