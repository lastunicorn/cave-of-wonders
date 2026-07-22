using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

public class AddPotLabelRequest : IRequest<AddPotLabelResponse>
{
	public PotFlexId PotId { get; set; }

	public string Label { get; set; }
}
