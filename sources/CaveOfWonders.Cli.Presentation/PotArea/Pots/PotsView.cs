// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Commando;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pots;

internal class PotsView : IView<PotsViewModel>
{
    public void Display(PotsViewModel potsViewModel)
    {
        if (potsViewModel.Culture != null)
        {
            CultureInfo.CurrentCulture = potsViewModel.Culture;
            CultureInfo.CurrentUICulture = potsViewModel.Culture;
        }

        DisplayCaveInstances(potsViewModel);
        DisplayTotals(potsViewModel);
        DisplayConversionRates(potsViewModel);
    }

    private static void DisplayCaveInstances(PotsViewModel potsViewModel)
    {
        PotsDataGrid potSnapshotControl = new()
        {
            Date = potsViewModel.Date,
            Values = potsViewModel.Values,
            Total = potsViewModel.Total
        };

        potSnapshotControl.Display();
    }

    private static void DisplayTotals(PotsViewModel potsViewModel)
    {
        Console.WriteLine();

        TotalsDataGrid totalsDataGrid = new()
        {
            Date = potsViewModel.Date,
            Values = potsViewModel.CurrencyTotals,
            Total = potsViewModel.Total
        };

        totalsDataGrid.Display();
    }

    private static void DisplayConversionRates(PotsViewModel potsViewModel)
    {
        Console.WriteLine();

        ExchangeRatesControl exchangeRatesControl = new()
        {
            ExchangeRates = potsViewModel.ConversionRates
        };

        exchangeRatesControl.Display();
    }
}
