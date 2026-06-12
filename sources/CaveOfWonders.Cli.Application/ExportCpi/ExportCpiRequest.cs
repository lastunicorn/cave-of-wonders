using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ExportCpi;

public class ExportCpiRequest : IRequest
{
    public string OutputPath { get; set; }
}
