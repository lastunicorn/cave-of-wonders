using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeleteSnapshots;

public class DeleteSnapshotsRequest
{
	public PotFlexId PotId { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public bool Confirmed { get; set; }
}
