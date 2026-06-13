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

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Wealth;

internal class WealthView : IView<WealthViewModel>
{
    public void Display(WealthViewModel wealthViewModel)
    {
        if (wealthViewModel.Culture != null)
        {
            CultureInfo.CurrentCulture = wealthViewModel.Culture;
            CultureInfo.CurrentUICulture = wealthViewModel.Culture;
        }

        DisplayCaveInstances(wealthViewModel);

        if (wealthViewModel.CurrencyTotalOverviews?.Count > 1)
            DisplayTotals(wealthViewModel);

        DisplayConversionRates(wealthViewModel);
    }

    private static void DisplayCaveInstances(WealthViewModel wealthViewModel)
    {
        PotsDataGrid potsDataGrid = new()
        {
            Date = wealthViewModel.Date,
            Values = wealthViewModel.Values,
            Total = wealthViewModel.Total
        };

        potsDataGrid.Display();
    }

    private static void DisplayTotals(WealthViewModel wealthViewModel)
    {
        Console.WriteLine();

        TotalsDataGrid totalsDataGrid = new()
        {
            Date = wealthViewModel.Date,
            Values = wealthViewModel.CurrencyTotalOverviews,
            Total = wealthViewModel.Total
        };

        totalsDataGrid.Display();
    }

    private static void DisplayConversionRates(WealthViewModel wealthViewModel)
    {
        Console.WriteLine();

        ExchangeRatesControl exchangeRatesControl = new()
        {
            ExchangeRates = wealthViewModel.ConversionRates
        };

        exchangeRatesControl.Display();
    }
}
