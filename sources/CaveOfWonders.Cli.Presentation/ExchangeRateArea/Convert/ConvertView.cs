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
        DisplayExchangeRateInformation(viewModel);
    }

    private static void DisplayConvertedValue(ConvertViewModel viewModel)
    {
        decimal initialValue = viewModel.InitialValue;
        string sourceCurrency = viewModel.SourceCurrency;

        decimal convertedValue = viewModel.ConvertedValue;
        string destinationCurrency = viewModel.DestinationCurrency;

        CustomConsole.WriteLineEmphasized($"{initialValue} {sourceCurrency} = {convertedValue:N2} {destinationCurrency}");
    }

    private static void DisplayExchangeRateInformation(ConvertViewModel viewModel)
    {
        string sourceCurrency = viewModel.SourceCurrency;

        string destinationCurrency = viewModel.DestinationCurrency;

        decimal exchangeValue = viewModel.ExchangeValue;
        DateTime exchangeDate = viewModel.ExchangeDate;

        CustomConsole.WriteLine("Conversion rate:");

        CustomConsole.Write($"  1 {sourceCurrency} = {exchangeValue:N4} {destinationCurrency} ");

        if (viewModel.IsActualDate)
            CustomConsole.WriteLine($"({exchangeDate:d})");
        else
            CustomConsole.WriteLine(ConsoleColor.DarkYellow, $"({exchangeDate:d})");
    }
}