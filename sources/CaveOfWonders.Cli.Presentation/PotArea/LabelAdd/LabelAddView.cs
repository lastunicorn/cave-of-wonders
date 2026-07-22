using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

internal class LabelAddView : IView<LabelAddViewModel>
{
	public void Display(LabelAddViewModel viewModel)
	{
		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = $"Added Label '{viewModel.Label}'";

		dataGrid.Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add("Pot");
		dataGrid.Columns.Add("Status");

		foreach (LabelAddItemViewModel item in viewModel.Items)
		{
			ShortPotId id = item.PotId;
			string status = item.WasAdded ? "Added" : "Already present";

			ContentRow row = new(id, item.PotName, status);

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
