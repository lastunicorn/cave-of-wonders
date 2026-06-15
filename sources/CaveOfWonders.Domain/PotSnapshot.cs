namespace DustInTheWind.CaveOfWonders.Domain;

public class PotSnapshot : IEquatable<PotSnapshot>
{
    public DateOnly Date { get; set; }

    public decimal Value { get; set; }
    
    public Pot Pot { get; init; }

    public bool Equals(PotSnapshot other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Date.Equals(other.Date) && Value.Equals(other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((PotSnapshot)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, Value);
    }

    public static bool operator ==(PotSnapshot left, PotSnapshot right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(PotSnapshot left, PotSnapshot right)
    {
        return !Equals(left, right);
    }
}