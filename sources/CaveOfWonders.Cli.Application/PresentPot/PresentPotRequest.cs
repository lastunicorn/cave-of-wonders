using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

public class PresentPotRequest
{
	public PotFlexId PotFlexId { get; set; }

	public bool IncludeInactivePots { get; set; }

	public bool? ShowDetails { get; set; }
}