namespace DustInTheWind.CaveOfWonders.DataTypes;

public record class PotIdentifier
{
    private readonly string partialValue;
    private readonly Guid? guid;

    public bool IsEmpty => !guid.HasValue && partialValue == null;

    public bool IsFullGuid => guid.HasValue;

    public static PotIdentifier Empty { get; } = new PotIdentifier();

    private PotIdentifier()
    {
    }

    public PotIdentifier(string value)
    {
        if (value is not null)
        {
            if (Guid.TryParse(value, out Guid g))
                guid = g;
            else
                partialValue = value;
        }
    }

    public PotIdentifier(Guid guid)
    {
        this.guid = guid;
    }

    public bool IsMatch(string text)
    {
        if (text == null) return false;

        if (guid.HasValue)
        {
            if (Guid.TryParse(text, out Guid textGuid))
                return guid.Value == textGuid;

            return false;
        }

        if (partialValue is not null)
        {
            int pos = text.IndexOf(partialValue, StringComparison.OrdinalIgnoreCase);
            return pos >= 0;
        }

        return false;
    }

    public override string ToString()
    {
        if (guid.HasValue)
            return guid.Value.ToString("D");

        if (partialValue is not null)
            return partialValue;

        return null;
    }

    public static implicit operator PotIdentifier(string value)
    {
        return new PotIdentifier(value);
    }

    public static implicit operator PotIdentifier(Guid guid)
    {
        return new PotIdentifier(guid);
    }

    public static implicit operator string(PotIdentifier potIdentifier)
    {
        return potIdentifier.ToString();
    }

    public static implicit operator Guid(PotIdentifier potIdentifier)
    {
        if (potIdentifier.guid.HasValue)
            return potIdentifier.guid.Value;

        throw new InvalidCastException("PotIdentifier does not contain a valid Guid.");
    }
}