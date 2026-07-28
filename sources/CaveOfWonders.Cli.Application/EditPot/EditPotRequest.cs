using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.EditPot;

public class EditPotRequest
{
	public PotFlexId PotId { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public string Currency { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }
}
