using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application;

public record class Amount
{
	public decimal Value { get; init; }

	public Currency Currency { get; init; }

	public static implicit operator decimal(Amount value)
	{
		return value.Value;
	}
}