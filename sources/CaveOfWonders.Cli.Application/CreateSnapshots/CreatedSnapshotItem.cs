using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CreateSnapshots;

public class CreatedSnapshotItem
{
	public DateOnly Date { get; init; }

	public Amount Value { get; init; }
}
