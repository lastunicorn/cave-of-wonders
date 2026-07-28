using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

public class PresentPotSnapshotsRequest
{
	public PotFlexId PotFlexId { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }
}
