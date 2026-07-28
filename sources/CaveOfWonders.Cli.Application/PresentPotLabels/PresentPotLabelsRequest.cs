using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

public class PresentPotLabelsRequest
{
	public PotFlexId PotFlexId { get; set; }

	public bool IncludeInactivePots { get; set; }
}
