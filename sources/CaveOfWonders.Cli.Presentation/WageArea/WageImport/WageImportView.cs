using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.WageImport;

internal class WageImportView : IView<WageImportViewModel>
{
	public void Display(WageImportViewModel viewModel)
	{
		DataGrid dataGrid = new DataGrid();

		dataGrid.Columns.Add("Name");
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

		dataGrid.Rows.Add("Total records", viewModel.TotalCount);
		dataGrid.Rows.Add("Added records", viewModel.AddedCount);
		dataGrid.Rows.Add("Updated records", viewModel.UpdatedCount);
		dataGrid.Rows.Add("Deleted records", viewModel.DeletedCount);

		dataGrid.Display();
	}
}