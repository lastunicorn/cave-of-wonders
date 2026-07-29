using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotDelete;

internal class SnapshotDeleteView : ViewBase<SnapshotDeleteViewModel>
{
	public override void Display(SnapshotDeleteViewModel viewModel)
	{
		string interval = BuildIntervalText(viewModel);

		if (viewModel.Cancelled)
			CustomConsole.WriteLineWarning("Snapshot deletion was cancelled.");
		else if (viewModel.DeletedCount == 0)
			CustomConsole.WriteLineWarning($"Pot '{viewModel.PotName}' has no snapshots to delete{interval}.");
		else
			CustomConsole.WriteLineSuccess($"{viewModel.DeletedCount} snapshots of pot '{viewModel.PotName}'{interval} were deleted successfully.");
	}

	private static string BuildIntervalText(SnapshotDeleteViewModel viewModel)
	{
		if (viewModel.StartDate != null && viewModel.EndDate != null)
			return $" between {viewModel.StartDate.Value:yyyy-MM-dd} and {viewModel.EndDate.Value:yyyy-MM-dd}";

		if (viewModel.StartDate != null)
			return $" starting with {viewModel.StartDate.Value:yyyy-MM-dd}";

		if (viewModel.EndDate != null)
			return $" up to {viewModel.EndDate.Value:yyyy-MM-dd}";

		return string.Empty;
	}
}
