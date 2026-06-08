using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Domain;

public class ExchangeRate : IEquatable<ExchangeRate>
{
    public DateTime Date { get; set; }

    public CurrencyPair CurrencyPair { get; set; }

    public decimal Value { get; set; }

    public decimal Convert(decimal value)
    {
        return value * Value;
    }

    public decimal ConvertBack(decimal value)
    {
        return value == 0
            ? 0
            : value / Value;
    }

    public ConversionAbility AnalyzeConversionAbility(CurrencyId source, CurrencyId destination)
    {
        bool canConvertDirect = CurrencyPair.Currency1 == source && CurrencyPair.Currency2 == destination;
        if (canConvertDirect)
            return ConversionAbility.ConvertDirect;

        bool canConvertReverse = CurrencyPair.Currency1 == destination && CurrencyPair.Currency2 == source;
        if (canConvertReverse)
            return ConversionAbility.ConvertReverse;

        return ConversionAbility.None;
    }

    public bool CanConvert(CurrencyId source, CurrencyId destination)
    {
        return (CurrencyPair.Currency1 == source && CurrencyPair.Currency2 == destination) ||
               (CurrencyPair.Currency1 == destination && CurrencyPair.Currency2 == source);
    }

    public ExchangeRate Invert()
    {
        return new ExchangeRate
        {
            CurrencyPair = CurrencyPair.Invert(),
            Date = Date,
            Value = ConvertBack(1)
        };
    }

    public bool Equals(ExchangeRate other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Date.Equals(other.Date) && CurrencyPair.Equals(other.CurrencyPair) && Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ExchangeRate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, CurrencyPair, Value);
    }

    public static bool operator ==(ExchangeRate left, ExchangeRate right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ExchangeRate left, ExchangeRate right)
    {
        return !Equals(left, right);
    }
}