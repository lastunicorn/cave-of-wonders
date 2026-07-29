using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotDelete;

internal class SnapshotDeleteView : ViewBase<SnapshotDeleteViewModel>
{
	public override void Display(SnapshotDeleteViewModel viewModel)
	{
		if (viewModel.Cancelled)
			CustomConsole.WriteLineWarning("Snapshot deletion was cancelled.");
		else if (viewModel.DeletedCount == 0)
			CustomConsole.WriteLineWarning($"Pot '{viewModel.PotName}' has no snapshots to delete.");
		else
			CustomConsole.WriteLineSuccess($"{viewModel.DeletedCount} snapshots of pot '{viewModel.PotName}' were deleted successfully.");
	}
}
