namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

public class PresentGemsResponse
{
	public IReadOnlyList<GemDto> Gems { get; init; }

	public Amount TotalAmount { get; init; }
}