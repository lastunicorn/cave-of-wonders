using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

internal class LabelView : ViewBase<LabelViewModel>
{
	public override void Display(LabelViewModel viewModel)
	{
		if (viewModel.Pots != null)
		{
			if (viewModel.Pots.Count == 0)
				CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
			else
				DisplayPots(viewModel);
		}
		else if (viewModel.Labels != null)
		{
			if (viewModel.Labels.Count == 0)
				CustomConsole.WriteLineWarning("There is no label.");
			else
				DisplayLabels(viewModel);
		}
	}

	private static void DisplayPots(LabelViewModel viewModel)
	{
		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = "Pot Labels";

		dataGrid.Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add("Pot");
		dataGrid.Columns.Add("Labels");

		foreach (PotLabelsViewModel item in viewModel.Pots)
		{
			ShortPotId id = item.PotId;
			string labels = string.Join(", ", item.Labels);

			ContentRow row = new(id, item.PotName, labels);

			if (!item.IsActive)
			{
				row[1].ForegroundColor = ConsoleColor.DarkGray;
				row[2].ForegroundColor = ConsoleColor.DarkGray;
			}

			dataGrid.Rows.Add(row);
		}

		dataGrid.Display();
	}

	private static void DisplayLabels(LabelViewModel viewModel)
	{
		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = "Labels";
		dataGrid.DisplayBorderBetweenRows = true;

		dataGrid.Columns.Add("Label");
		dataGrid.Columns.Add("Pots");

		foreach (LabelPotsViewModel item in viewModel.Labels)
		{
			dataGrid.Rows.Add(
				new ContentCell(item.Label),
				new ContentCell(item.PotNames));
		}

		dataGrid.Display();
	}
}