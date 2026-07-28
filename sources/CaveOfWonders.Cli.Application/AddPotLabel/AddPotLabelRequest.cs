using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

public class AddPotLabelRequest
{
	public PotFlexId PotId { get; set; }

	public string Label { get; set; }
}
