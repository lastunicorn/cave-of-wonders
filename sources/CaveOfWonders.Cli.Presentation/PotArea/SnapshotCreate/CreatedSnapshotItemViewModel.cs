using DustInTheWind.CaveOfWonders.Cli.Application.CreateSnapshots;
using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotCreate;

internal class CreatedSnapshotItemViewModel
{
	public DateOnly Date { get; }

	public Amount Value { get; }

	internal CreatedSnapshotItemViewModel(CreatedSnapshotItem item)
	{
		Date = item.Date;
		Value = item.Value;
	}
}
