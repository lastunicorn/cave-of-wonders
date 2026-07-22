using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.CpiImport;

internal class CpiImportView : IView<CpiImportViewModel>
{
	public void Display(CpiImportViewModel viewModel)
	{
		CustomConsole.WriteLineSuccess("Execute succeeded.");

		CustomConsole.WriteLine($"  Added: {viewModel.AddedCount}");
		CustomConsole.WriteLine($"  Updated: {viewModel.UpdatedCount}");
		CustomConsole.WriteLine($"  Total: {viewModel.TotalCount}");
	}
}