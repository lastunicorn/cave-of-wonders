namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class FileImportResult
{
	public string FilePath { get; internal set; }

	public int UpdatedGemCount { get; internal set; }

	public int AddedGemCount { get; internal set; }

	public int SkippedGemCount { get; internal set; }
}