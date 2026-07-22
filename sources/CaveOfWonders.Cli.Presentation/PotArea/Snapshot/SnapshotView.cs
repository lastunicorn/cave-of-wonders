using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Snapshot;

internal class SnapshotView : ViewBase<SnapshotViewModel>
{
	public override void Display(SnapshotViewModel viewModel)
	{
		if (viewModel.PotSummaries?.Count > 0)
			DisplayMatchedPots(viewModel.PotSummaries);
		else if (viewModel.Pot != null)
			DisplaySnapshots(viewModel.Pot, viewModel.Snapshots);
		else
			CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
	}

	private static void DisplayMatchedPots(List<PotSummaryViewModel> potSummaries)
	{
		CustomConsole.WriteLineWarning("Multiple pots match the specified name or id:");

		DataGrid dataGrid = DataGridTemplate.CreateNew();

		dataGrid.Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add("Name");
		dataGrid.Columns.Add("Currency");

		foreach (PotSummaryViewModel potSummary in potSummaries)
		{
			ShortPotId id = potSummary.Id;
			ContentRow row = new(id, potSummary.Name, potSummary.Currency);

			if (!potSummary.IsActive)
			{
				row[1].ForegroundColor = ConsoleColor.DarkGray;
				row[2].ForegroundColor = ConsoleColor.DarkGray;
			}

			dataGrid.Rows.Add(row);
		}

		dataGrid.Display();
	}

	private static void DisplaySnapshots(PotSummaryViewModel pot, List<PotSnapshotViewModel> snapshots)
	{
		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = $"Snapshots - {pot.Name}";

		dataGrid.Columns.Add("Date");
		dataGrid.Columns.Add(new Column("Value")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});

		foreach (PotSnapshotViewModel snapshot in snapshots)
			dataGrid.Rows.Add(snapshot.Date.ToString("d"), snapshot.Value.ToDisplayString());

		dataGrid.Display();
	}
}
