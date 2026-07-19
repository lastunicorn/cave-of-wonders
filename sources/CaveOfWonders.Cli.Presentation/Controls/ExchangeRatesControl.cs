using DustInTheWind.ConsoleTools.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

internal class ExchangeRatesControl : BlockControl
{
	public List<ExchangeRateViewModel> ExchangeRates { get; set; }

	protected override void DoDisplayContent(ControlDisplay display)
	{
		display.WriteRow("Conversion Rates:");

		foreach (ExchangeRateViewModel exchangeRate in ExchangeRates)
		{
			display.StartRow();
			display.Write("  ");

			ExchangeRateControl exchangeRateControl = new()
			{
				ViewModel = exchangeRate
			};
			exchangeRateControl.Display();

			display.EndRow();
		}
	}
}