namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class ImportGemsResponse
{
	public int UpdatedGemCount { get; internal set; }

	public int AddedGemCount { get; internal set; }

	public int SkippedGemCount { get; set; }

	public int TotalGemCount { get; internal set; }

	public TimeSpan Duration { get; internal set; }
}