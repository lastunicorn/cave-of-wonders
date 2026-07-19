using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeletePot;

public class DeletePotRequest : IRequest<DeletePotResponse>
{
    public PotFlexId PotId { get; set; }
}
