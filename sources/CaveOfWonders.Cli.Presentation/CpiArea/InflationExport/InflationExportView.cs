using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.InflationExport;

internal class InflationExportView : IView<InflationExportCommand>
{
	public void Display(InflationExportCommand viewModel)
	{
		CustomConsole.WriteLineSuccess("Export succeeded.");
	}
}