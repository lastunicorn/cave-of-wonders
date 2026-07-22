namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Snapshot;

internal class SnapshotViewModel
{
	public List<PotSummaryViewModel> PotSummaries { get; init; }

	public PotSummaryViewModel Pot { get; init; }

	public List<PotSnapshotViewModel> Snapshots { get; init; }
}
