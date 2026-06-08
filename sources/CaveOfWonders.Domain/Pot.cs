namespace DustInTheWind.CaveOfWonders.Domain;

public class Pot
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public uint DisplayOrder { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Currency { get; set; }

    public List<Gem> Gems { get; } = [];
    
    public List<string> Labels { get; } = [];

    public bool IsActive(DateTime date)
    {
        return date >= StartDate && (EndDate == null || date <= EndDate);
    }

    public Gem GetLastGem()
    {
        bool hasGems = Gems.Count > 0;

        return hasGems
            ? Gems[Gems.Count - 1]
            : null;
    }
}