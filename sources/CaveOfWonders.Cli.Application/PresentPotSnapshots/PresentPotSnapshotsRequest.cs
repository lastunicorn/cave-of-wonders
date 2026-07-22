using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

public class PresentPotSnapshotsRequest : IRequest<PresentPotSnapshotsResponse>
{
	public PotFlexId PotFlexId { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }
}
