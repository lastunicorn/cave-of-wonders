using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ExportInflation;

public class ExportInflationRequest : IRequest
{
    public string OutputPath { get; set; }
}
