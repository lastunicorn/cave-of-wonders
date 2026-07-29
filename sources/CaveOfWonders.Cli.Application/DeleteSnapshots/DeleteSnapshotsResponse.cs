namespace DustInTheWind.CaveOfWonders.Cli.Application.DeleteSnapshots;

public class DeleteSnapshotsResponse
{
	public string PotName { get; set; }

	public int DeletedCount { get; set; }

	public bool Cancelled { get; set; }
}
