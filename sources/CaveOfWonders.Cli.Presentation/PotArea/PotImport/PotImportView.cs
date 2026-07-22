using DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotImport;

internal class PotImportView : IView<PotImportViewModel>
{
	public void Display(PotImportViewModel viewModel)
	{
		DisplayReports(viewModel.Report);
	}

	private static void DisplayReports(IEnumerable<PotImportReport> report)
	{
		ReportDataGrid dataGrid = new()
		{
			Title = "Import Pot Snapshots Report",
			TitleRow =
			{
				BackgroundColor = ConsoleColor.Gray,
				ForegroundColor = ConsoleColor.Black
			},
			Border =
			{
				ForegroundColor = ConsoleColor.DarkGray
			},
			HeaderRow =
			{
				ForegroundColor = ConsoleColor.White
			},
		};

		dataGrid.AddRows(report);

		dataGrid.Display();
	}
}