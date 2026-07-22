using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Snapshot;

internal class PotSnapshotViewModel
{
	public DateOnly Date { get; }

	public Amount Value { get; }

	public PotSnapshotViewModel(PotSnapshotItem potSnapshotItem)
	{
		Date = potSnapshotItem.Date;
		Value = potSnapshotItem.Value;
	}
}
