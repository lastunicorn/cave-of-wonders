namespace DustInTheWind.CaveOfWonders.Cli.Application;

public record class DatedAmount
{
	public DateOnly Date { get; init; }

	public decimal Value { get; init; }

	public string Currency { get; init; }
}