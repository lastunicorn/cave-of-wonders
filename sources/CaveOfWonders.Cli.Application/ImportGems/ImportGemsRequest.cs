using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class ImportGemsRequest
{
	public string FilePath { get; set; }

	public FileType FileType { get; set; }

	public PotFlexId PotFlexId { get; set; }
}