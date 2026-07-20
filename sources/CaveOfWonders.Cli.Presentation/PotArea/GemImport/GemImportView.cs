using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.GemImport;

internal class GemImportView : IView<GemImportViewModel>
{
	public void Display(GemImportViewModel viewModel)
	{
		DataGrid dataGrid = new();

		dataGrid.Columns.Add("Name");
		dataGrid.Columns.Add("Value");

		dataGrid.Rows.Add("Added", viewModel.AddedGemCount);
		dataGrid.Rows.Add("Updated", viewModel.UpdatedGemCount);
		dataGrid.Rows.Add("Skipped", viewModel.SkippedGemCount);
		dataGrid.Rows.Add("Total", viewModel.TotalGemCount);

		dataGrid.Display();
		
		CustomConsole.WriteLine();
		CustomConsole.WriteLine(ConsoleColor.Gray, $"Duration: {viewModel.Duration.TotalSeconds:0.00} s");
	}
}