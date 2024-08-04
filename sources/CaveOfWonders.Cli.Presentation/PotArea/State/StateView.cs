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

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;

internal class StateView : IView<StateViewModel>
{
    public void Display(StateViewModel stateViewModel)
    {
        DisplayCaveInstances(stateViewModel);
        DisplayConversionRates(stateViewModel);
    }

    private void DisplayCaveInstances(StateViewModel stateViewModel)
    {
        PotSnapshotControl potSnapshotControl = new()
        {
            Date = stateViewModel.Date,
            Values = stateViewModel.Values,
            Total = stateViewModel.Total
        };

        potSnapshotControl.Display();
    }

    private void DisplayConversionRates(StateViewModel stateViewModel)
    {
        Console.WriteLine();
        CustomConsole.WriteLine("Conversion Rates:");

        foreach (ExchangeRateViewModel conversionRate in stateViewModel.ConversionRates)
        {
            CustomConsole.Write($"  1 {conversionRate.SourceCurrency} = {conversionRate.Value} {conversionRate.DestinationCurrency}");

            if (!conversionRate.IsCurrent)
                CustomConsole.Write(ConsoleColor.DarkYellow, $" ({conversionRate.CurrencyDate:d})");

            CustomConsole.WriteLine();
        }
    }
}