namespace DustInTheWind.CaveOfWonders.DataTypes;

/// <remarks>
/// ISO 4217 - Currency codes
/// </remarks>
public readonly record struct Currency
{
	public static readonly Currency RON = new("RON");
	public static readonly Currency AUD = new("AUD");
	public static readonly Currency BGN = new("BGN");
	public static readonly Currency CAD = new("CAD");
	public static readonly Currency CHF = new("CHF");
	public static readonly Currency CZK = new("CZK");
	public static readonly Currency DKK = new("DKK");
	public static readonly Currency EGP = new("EGP");
	public static readonly Currency EUR = new("EUR");
	public static readonly Currency GBP = new("GBP");
	public static readonly Currency HUF = new("HUF");
	public static readonly Currency JPY = new("JPY");
	public static readonly Currency MDL = new("MDL");
	public static readonly Currency NOK = new("NOK");
	public static readonly Currency PLN = new("PLN");
	public static readonly Currency RUB = new("RUB");
	public static readonly Currency SEK = new("SEK");
	public static readonly Currency SKK = new("SKK");
	public static readonly Currency TRY = new("TRY");
	public static readonly Currency USD = new("USD");
	public static readonly Currency XAU = new("XAU");
	public static readonly Currency XDR = new("XDR");
	public static readonly Currency AED = new("AED");
	public static readonly Currency BRL = new("BRL");
	public static readonly Currency CNY = new("CNY");
	public static readonly Currency HRK = new("HRK");
	public static readonly Currency INR = new("INR");
	public static readonly Currency KRW = new("KRW");
	public static readonly Currency MXN = new("MXN");
	public static readonly Currency NZD = new("NZD");
	public static readonly Currency RSD = new("RSD");
	public static readonly Currency THB = new("THB");
	public static readonly Currency UAH = new("UAH");
	public static readonly Currency ZAR = new("ZAR");

	public static readonly IReadOnlySet<Currency> KnownValues = new HashSet<Currency>
	{
		RON,
		AUD,
		BGN,
		CAD,
		CHF,
		CZK,
		DKK,
		EGP,
		EUR,
		GBP,
		HUF,
		JPY,
		MDL,
		NOK,
		PLN,
		RUB,
		SEK,
		SKK,
		TRY,
		USD,
		XAU,
		XDR,
		AED,
		BRL,
		CNY,
		HRK,
		INR,
		KRW,
		MXN,
		NZD,
		RSD,
		THB,
		UAH,
		ZAR
	};

	private readonly string value;

	public static Currency Empty { get; } = new();

	public bool IsEmpty => value == null;

	private Currency(string value)
	{
		if (value == null) throw new ArgumentNullException(nameof(value));
		if (value.Length != 3) throw new ArgumentException("The currency ID must have three characters.", nameof(value));

		this.value = value.ToUpperInvariant();
	}

	public override string ToString()
	{
		return value;
	}

	public static implicit operator Currency(string value)
	{
		return value == null
			? Empty
			: new Currency(value);
	}

	public static implicit operator string(Currency currency)
	{
		return currency.value;
	}
}