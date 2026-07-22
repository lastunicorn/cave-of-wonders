using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.EditPot;

public class EditPotRequest : IRequest<EditPotResponse>
{
	public PotFlexId PotId { get; set; }

	public string Name { get; set; }
}
