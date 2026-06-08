namespace DustInTheWind.CaveOfWonders.Infrastructure;

public readonly struct CurrencyId
{
    private readonly string value;

    public static CurrencyId Empty { get; } = new();

    public bool IsEmpty => value == null;

    private CurrencyId(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }

    public static implicit operator CurrencyId(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value.Length != 3) throw new ArgumentException("The currency ID must have three characters.", nameof(value));

        return new CurrencyId(value.ToUpper());
    }

    public static implicit operator string(CurrencyId currencyId)
    {
        return currencyId.value;
    }
}