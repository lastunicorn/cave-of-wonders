using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Domain;

public record class Gem
{
    public DateTime Date { get; set; }

    public GemCategory Category { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public Dictionary<string, string> Parameters { get; } = [];

    public Pot Pot { get; set; }

    public virtual bool Equals(Gem other)
    {
        if (other == null) return false;

        return Date == other.Date
            && Category == other.Category
            && Amount == other.Amount
            && Description == other.Description
            && Parameters.SequenceEqual(other.Parameters)
            && Pot?.Id == other.Pot?.Id;
    }
}