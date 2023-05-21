namespace DustInTheWind.CaveOfWonders.Domain;

public class Pot
{
    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Currency { get; set; }

    public List<Gem> Gems { get; } = new();

    public bool IsActive(DateTime date)
    {
        return date >= StartDate && (EndDate == null || date <= EndDate);
    }
}