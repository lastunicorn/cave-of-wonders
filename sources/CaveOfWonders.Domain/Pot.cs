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

    public List<PotSnapshot> Snapshots { get; } = [];
    
    public List<string> Labels { get; } = [];

    public bool IsActive(DateOnly date)
    {
        return date >= StartDate && (EndDate == null || date <= EndDate);
    }

    public PotSnapshot GetLastSnapshot()
    {
        bool hasSnapshots = Snapshots.Count > 0;

        return hasSnapshots
            ? Snapshots[Snapshots.Count - 1]
            : null;
    }
}