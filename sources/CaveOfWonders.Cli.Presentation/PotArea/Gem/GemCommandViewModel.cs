using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gem;

internal class GemCommandViewModel
{
	public IReadOnlyList<GemDto> Gems { get; init; }

	public Amount TotalAmount { get; init; }
}