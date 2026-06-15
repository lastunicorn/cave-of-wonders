using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class ImportGemsRequest : IRequest<ImportGemsResponse>
{
    public string FilePath { get; set; }
}