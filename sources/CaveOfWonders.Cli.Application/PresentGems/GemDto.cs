using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

public class GemDto
{
	public DateTime Date { get; set; }

	public GemCategory Category { get; set; }

	public Amount Amount { get; set; }
}