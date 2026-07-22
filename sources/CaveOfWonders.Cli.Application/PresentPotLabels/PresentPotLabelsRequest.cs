using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

public class PresentPotLabelsRequest : IRequest<PresentPotLabelsResponse>
{
	public PotFlexId PotFlexId { get; set; }

	public bool IncludeInactivePots { get; set; }
}
