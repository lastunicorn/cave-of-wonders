using DustInTheWind.CaveOfWonders.DataTypes;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class ImportGemsRequest : IRequest<ImportGemsResponse>
{
    public string FilePath { get; set; }

    public FileType FileType { get; set; }

    public PotFlexId PotFlexId { get; set; }
}