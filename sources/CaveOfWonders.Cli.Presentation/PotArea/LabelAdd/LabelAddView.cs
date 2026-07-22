using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

internal class LabelAddView : IView<LabelAddViewModel>
{
	public void Display(LabelAddViewModel viewModel)
	{
		CustomConsole.WriteLineSuccess($"Label '{viewModel.Label}' was added to the following pots:");

		DataGrid dataGrid = DataGridTemplate.CreateNew();

		dataGrid.Columns.Add(new Column("Id")
		{
			ForegroundColor = ConsoleColor.DarkGray,
			CellContentOverflow = CellContentOverflow.PreserveOverflow
		});

		dataGrid.Columns.Add("Pot");

		foreach (LabelAddItemViewModel item in viewModel.Items)
		{
			ShortPotId id = item.PotId;
			dataGrid.Rows.Add(id, item.PotName);
		}

		dataGrid.Display();
	}
}
