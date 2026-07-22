using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application;

public readonly struct CurrencyPairDto
{
	public Currency Currency1 { get; init; }

	public Currency Currency2 { get; init; }
}