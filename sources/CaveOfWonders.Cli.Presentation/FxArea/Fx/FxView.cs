using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.Fx;

internal class FxView : IView<FxViewModel>
{
	public void Display(FxViewModel viewModel)
	{
		if (viewModel.DailyExchangeRates.Count == 0 || viewModel.DailyExchangeRates.All(x => x.ExchangeRates.Count == 0))
			CustomConsole.WriteLineWarning("There are no exchange rates.");
		else
			DisplayExchangeRates(viewModel);

		if (viewModel.Comments != null)
			CustomConsole.WriteLineWarning(viewModel.Comments);
	}

	private static void DisplayExchangeRates(FxViewModel viewModel)
	{
		FxDataGrid dataGrid = new(viewModel);
		dataGrid.Display();
	}
}