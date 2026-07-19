using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

internal class ExchangeRateControl : InlineControl
{
	public ExchangeRateViewModel ViewModel { get; set; }

	protected override void DoDisplayContent()
	{
		if (ViewModel == null)
			return;

		Console.Write($"1 {ViewModel.SourceCurrency} = {ViewModel.Value:N4} {ViewModel.DestinationCurrency}");

		if (!ViewModel.IsCurrent)
			CustomConsole.Write(ConsoleColor.DarkYellow, $" ({ViewModel.CurrencyDate:d})");
	}
}