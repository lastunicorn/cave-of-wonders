using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.Fx;

internal class FxView : IView<PresentExchangeRateResponse>
{
	public void Display(PresentExchangeRateResponse response)
	{
		if (response.DailyExchangeRates.Count == 0 || response.DailyExchangeRates.All(x => x.ExchangeRates.Count == 0))
			CustomConsole.WriteLineWarning($"There are no exchange rates.");
		else
			DisplayExchangeRates(response);

		if (response.Comments != null)
			CustomConsole.WriteLineWarning(response.Comments);
	}

	private static void DisplayExchangeRates(PresentExchangeRateResponse response)
	{
		FxDataGrid dataGrid = new(response);
		dataGrid.Display();
	}
}