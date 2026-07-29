namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotDelete;

internal class SnapshotDeleteViewModel
{
	public string PotName { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public int DeletedCount { get; set; }

	public bool Cancelled { get; set; }
}
