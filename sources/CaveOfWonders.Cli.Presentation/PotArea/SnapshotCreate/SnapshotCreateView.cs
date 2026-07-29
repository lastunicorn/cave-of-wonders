using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotCreate;

internal class SnapshotCreateView : IView<SnapshotCreateViewModel>
{
	public void Display(SnapshotCreateViewModel viewModel)
	{
		if (viewModel.Items.Count == 0)
		{
			CustomConsole.WriteLineWarning($"No snapshots were created for pot '{viewModel.PotName}'.");
			return;
		}

		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = $"Created Snapshots - {viewModel.PotName}";

		dataGrid.Columns.Add("Date");
		dataGrid.Columns.Add(new Column("Value")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});

		foreach (CreatedSnapshotItemViewModel item in viewModel.Items)
			dataGrid.Rows.Add(item.Date.ToString("d"), item.Value.ToDisplayString());

		dataGrid.Display();
	}
}
