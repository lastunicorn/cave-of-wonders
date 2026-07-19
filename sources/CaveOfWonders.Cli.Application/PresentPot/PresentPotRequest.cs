using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

public class PresentPotRequest : IRequest<PresentPotResponse>
{
	public PotFlexId PotFlexId { get; set; }

	public bool IncludeInactivePots { get; set; }

	public bool? ShowDetails { get; set; }
}