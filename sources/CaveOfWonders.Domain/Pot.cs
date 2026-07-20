namespace DustInTheWind.CaveOfWonders.Domain;

public class Pot
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public uint DisplayOrder { get; set; }

	public DateOnly StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public string Currency { get; set; }

	public PotSnapshotCollection Snapshots { get; }

	public List<PotLabel> Labels { get; } = [];

	public Pot()
	{
		Snapshots = new PotSnapshotCollection(this);
	}

	public bool IsActive(DateOnly date)
	{
		return date >= StartDate && (EndDate == null || date <= EndDate);
	}
}