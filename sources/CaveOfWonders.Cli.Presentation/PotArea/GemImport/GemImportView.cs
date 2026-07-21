using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.GemImport;

internal class GemImportView : IView<GemImportViewModel>
{
	public void Display(GemImportViewModel viewModel)
	{
		DisplayOverview(viewModel);

		foreach (FileImportResult fileImportResult in viewModel.FileImportResults)
			Display(fileImportResult);

		CustomConsole.WriteLine();
		CustomConsole.WriteLine(ConsoleColor.Gray, $"Duration: {viewModel.Duration.TotalSeconds:0.00} s");
	}

	private static void DisplayOverview(GemImportViewModel viewModel)
	{
		DataGrid dataGrid = new()
		{
			Title = "Import Overview",
			Footer = new[]
			{
				$"Files: {viewModel.FileImportResults?.Count.ToString("N0") ?? "0"}", 
				$"Total Gems: {viewModel.TotalGemCount:N0}"
			}
		};

		dataGrid.Columns.Add("Name");
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

		dataGrid.Rows.Add("Added", viewModel.AddedGemCount.ToString("N0"));
		dataGrid.Rows.Add("Updated", viewModel.UpdatedGemCount.ToString("N0"));
		dataGrid.Rows.Add("Skipped", viewModel.SkippedGemCount.ToString("N0"));

		dataGrid.Display();
	}

	private void Display(FileImportResult fileImportResult)
	{
		Console.WriteLine();
		Console.WriteLine($"File: {fileImportResult.FilePath}");

		DataGrid dataGrid = new();

		dataGrid.Columns.Add("Name");
		dataGrid.Columns.Add("Value", HorizontalAlignment.Right);

		dataGrid.Rows.Add("Added", fileImportResult.AddedGemCount.ToString("N0"));
		dataGrid.Rows.Add("Updated", fileImportResult.UpdatedGemCount.ToString("N0"));
		dataGrid.Rows.Add("Skipped", fileImportResult.SkippedGemCount.ToString("N0"));

		int totalGemCount = fileImportResult.AddedGemCount + fileImportResult.UpdatedGemCount + fileImportResult.SkippedGemCount;
		dataGrid.Footer = new[]
		{
			$"Total Gems: {totalGemCount:N0}"
		};

		dataGrid.Display();
	}
}