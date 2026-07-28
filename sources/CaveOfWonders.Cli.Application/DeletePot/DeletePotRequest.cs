using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeletePot;

public class DeletePotRequest
{
	public PotFlexId PotId { get; set; }

	public bool Confirmed { get; set; }
}