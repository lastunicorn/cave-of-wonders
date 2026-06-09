namespace DustInTheWind.CaveOfWonders.Domain;

public class Gem : IEquatable<Gem>
{
    public DateOnly Date { get; set; }

    public decimal Value { get; set; }

    public bool Equals(Gem other)
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
        return Equals((Gem)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, Value);
    }

    public static bool operator ==(Gem left, Gem right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Gem left, Gem right)
    {
        return !Equals(left, right);
    }
}