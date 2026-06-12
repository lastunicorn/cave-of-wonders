namespace DustInTheWind.CaveOfWonders.DataTypes;

public readonly record struct Currency
{
    private readonly string value;

    public static Currency Empty { get; } = new();

    public bool IsEmpty => value == null;

    private Currency(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }

    public static implicit operator Currency(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value.Length != 3) throw new ArgumentException("The currency ID must have three characters.", nameof(value));

        return new Currency(value.ToUpper());
    }

    public static implicit operator string(Currency currency)
    {
        return currency.value;
    }
}