using DustInTheWind.CaveOfWonders.Infrastructure;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

public class GainRequest : IRequest<GainResponse>
{
    public MonthDate Month { get; set; }
}