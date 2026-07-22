using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

internal class LabelView : IView<LabelViewModel>
{
	public void Display(LabelViewModel viewModel)
	{
		if (viewModel.Items.Count == 0)
		{
			CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
			return;
		}

		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = "Pot Labels";

		dataGrid.Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add("Pot");
		dataGrid.Columns.Add("Labels");

		foreach (PotLabelsItemViewModel item in viewModel.Items)
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
}
