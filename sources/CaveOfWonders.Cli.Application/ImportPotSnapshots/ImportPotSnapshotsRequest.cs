namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots;

public class ImportPotSnapshotsRequest
{
	public string SourceFilePath { get; set; }

	public string MappingsFilePath { get; set; }

	public bool Overwrite { get; set; }
}