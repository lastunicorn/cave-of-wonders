namespace DustInTheWind.CaveOfWonders.Cli.Application;

public record class DatedAmount
{
	public DateOnly Date { get; init; }

	public decimal Value { get; init; }

	public string Currency { get; init; }

	public static DatedAmount operator +(DatedAmount datedAmount, DatedAmount b)
	{
		if (datedAmount == null)
			return b;

		if (b == null)
			return datedAmount;

		if (datedAmount.Currency != b.Currency)
			throw new InvalidOperationException($"Cannot add amounts with different currencies. A: {datedAmount.Currency}, B: {b.Currency}");

		return new DatedAmount
		{
			Date = datedAmount.Date,
			Value = datedAmount.Value + b.Value,
			Currency = datedAmount.Currency
		};
	}

	public static DatedAmount operator +(DatedAmount datedAmount, decimal value)
	{
		if (datedAmount == null) throw new ArgumentNullException(nameof(datedAmount));

		return new DatedAmount
		{
			Date = datedAmount.Date,
			Value = datedAmount.Value + value,
			Currency = datedAmount.Currency
		};
	}
}