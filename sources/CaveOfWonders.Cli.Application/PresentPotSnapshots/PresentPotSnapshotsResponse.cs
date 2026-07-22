namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

public class PresentPotSnapshotsResponse
{
	public List<PotSummary> PotSummaries { get; set; }

	public PotSummary Pot { get; set; }

	public List<PotSnapshotItem> Snapshots { get; set; }
}
