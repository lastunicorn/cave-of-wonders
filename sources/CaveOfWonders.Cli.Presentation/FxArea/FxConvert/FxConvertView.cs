using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxConvert;

internal class FxConvertView : IView<FxConvertViewModel>
{
	public void Display(FxConvertViewModel viewModel)
	{
		CustomConsole.WriteLine();
		DisplayConvertedValue(viewModel);

		CustomConsole.WriteLine();
		DisplayExchangeRateInformation(viewModel.ExchangeRate);
	}

	private static void DisplayConvertedValue(FxConvertViewModel viewModel)
	{
		decimal initialValue = viewModel.InitialValue;
		string sourceCurrency = viewModel.SourceCurrency;

		decimal convertedValue = viewModel.ConvertedValue;
		string destinationCurrency = viewModel.DestinationCurrency;

		CustomConsole.WriteLineEmphasized($"{initialValue} {sourceCurrency} = {convertedValue:N2} {destinationCurrency}");
	}

	private static void DisplayExchangeRateInformation(ExchangeRateViewModel exchangeRateViewModel)
	{
		ExchangeRatesControl exchangeRatesControl = new()
		{
			ExchangeRates =
			[
				exchangeRateViewModel
			],
			ForegroundColor = ConsoleColor.DarkGray
		};

		exchangeRatesControl.Display();
	}
}