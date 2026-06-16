using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

public class PresentGemsRequest : IRequest<PresentGemsResponse>
{
    public string PotId { get; set; }

    public DateOnly? Date { get; set; }
}