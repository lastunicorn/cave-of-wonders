namespace DustInTheWind.CaveOfWonders.DataTypes;

public record class PotFlexId
{
    private readonly string partialValue;
    private readonly Guid? guid;

    public bool IsEmpty => !guid.HasValue && partialValue == null;

    public bool IsFullGuid => guid.HasValue;

    public static PotFlexId Empty { get; } = new();

    private PotFlexId()
    {
    }

    public PotFlexId(string value)
    {
        if (value is null) 
            return;
        
        if (Guid.TryParse(value, out Guid g))
            guid = g;
        else
            partialValue = value;
    }

    public PotFlexId(Guid guid)
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
        return guid.HasValue
            ? guid.Value.ToString("D")
            : partialValue;
    }

    public static implicit operator PotFlexId(string value)
    {
        return new PotFlexId(value);
    }

    public static implicit operator PotFlexId(Guid guid)
    {
        return new PotFlexId(guid);
    }

    public static implicit operator string(PotFlexId potFlexId)
    {
        return potFlexId.ToString();
    }

    public static implicit operator Guid(PotFlexId potFlexId)
    {
        if (potFlexId.guid.HasValue)
            return potFlexId.guid.Value;

        throw new InvalidCastException("PotIdentifier does not contain a valid Guid.");
    }
}