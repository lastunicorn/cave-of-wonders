// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Convert;

internal class ConvertView : IView<ConvertViewModel>
{
    public void Display(ConvertViewModel viewModel)
    {
        CustomConsole.WriteLine();
        DisplayConvertedValue(viewModel);

        CustomConsole.WriteLine();
        DisplayExchangeRateInformation(viewModel.ExchangeRate);
    }

    private static void DisplayConvertedValue(ConvertViewModel viewModel)
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
            ExchangeRates = new List<ExchangeRateViewModel>
            {
                exchangeRateViewModel
            },
            ForegroundColor = ConsoleColor.DarkGray
        };

        exchangeRatesControl.Display();
    }
}