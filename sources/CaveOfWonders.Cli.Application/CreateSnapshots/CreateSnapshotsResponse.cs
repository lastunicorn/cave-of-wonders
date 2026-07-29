namespace DustInTheWind.CaveOfWonders.Cli.Application.CreateSnapshots;

public class CreateSnapshotsResponse
{
	public Guid PotId { get; set; }

	public string PotName { get; set; }

	public List<CreatedSnapshotItem> Items { get; init; } = [];
}
