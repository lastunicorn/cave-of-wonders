using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.GemImport;

internal class GemImportViewModel
{
	public List<FileImportResult> FileImportResults { get; init; }

	public int UpdatedGemCount { get; init; }

	public int AddedGemCount { get; init; }

	public int SkippedGemCount { get; init; }

	public int TotalGemCount { get; init; }

	public TimeSpan Duration { get; init; }
}