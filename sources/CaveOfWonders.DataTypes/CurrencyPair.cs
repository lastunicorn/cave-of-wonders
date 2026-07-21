using System.Text.RegularExpressions;

namespace DustInTheWind.CaveOfWonders.DataTypes;

public readonly record struct CurrencyPair
{
	private static readonly Regex Regex = new(@"^(.{3})[\/| ]?(.{3})$", RegexOptions.Singleline);

	public static CurrencyPair Empty { get; } = new();

	public Currency Currency1 { get; init; }

	public Currency Currency2 { get; init; }

	public bool IsEmpty => Currency1.IsEmpty || Currency2.IsEmpty;

	public CurrencyPair()
	{
		Currency1 = Currency.Empty;
		Currency2 = Currency.Empty;
	}

	public CurrencyPair(Currency currency1, Currency currency2)
	{
		Currency1 = currency1;
		Currency2 = currency2;
	}

	public CurrencyPair(string value)
	{
		if (value == null) throw new ArgumentNullException(nameof(value));

		Match match = Regex.Match(value);

		if (!match.Success)
			throw new ArgumentException($"Invalid exchange pair identifier: {value}.", nameof(value));

		Currency1 = match.Groups[1].Value;
		Currency2 = match.Groups[2].Value;
	}

	public CurrencyPair Invert()
	{
		return new CurrencyPair(Currency2, Currency1);
	}

	public override string ToString()
	{
		return Currency1 + Currency2;
	}

	public bool Equals(CurrencyPair other)
	{
		return Currency1.Equals(other.Currency1) &&
			Currency2.Equals(other.Currency2);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Currency1, Currency2);
	}

	public static implicit operator CurrencyPair(string value)
	{
		return value == null
			? Empty
			: new CurrencyPair(value);
	}

	public static implicit operator string(CurrencyPair currencyPair)
	{
		return currencyPair.ToString();
	}

	public static implicit operator CurrencyPair((Currency currency1, Currency currency2) tuple)
	{
		return new CurrencyPair(tuple.currency1, tuple.currency2);
	}

	public static implicit operator CurrencyPair((string currency1, string currency2) tuple)
	{
		return new CurrencyPair(tuple.currency1, tuple.currency2);
	}

	public static implicit operator CurrencyPair?((string currency1, string currency2) tuple)
	{
		if (tuple.currency1 == null || tuple.currency2 == null)
			return null;

		return new CurrencyPair(tuple.currency1, tuple.currency2);
	}
}