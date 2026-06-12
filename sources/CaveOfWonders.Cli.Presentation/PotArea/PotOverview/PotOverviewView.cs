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

using System.Globalization;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Commando;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotOverview;

internal class PotOverviewView : IView<PotOverviewViewModel>
{
    public void Display(PotOverviewViewModel potOverviewViewModel)
    {
        if (potOverviewViewModel.Culture != null)
        {
            CultureInfo.CurrentCulture = potOverviewViewModel.Culture;
            CultureInfo.CurrentUICulture = potOverviewViewModel.Culture;
        }

        DisplayCaveInstances(potOverviewViewModel);

        if (potOverviewViewModel.CurrencyTotalOverviews?.Count > 1)
            DisplayTotals(potOverviewViewModel);

        DisplayConversionRates(potOverviewViewModel);
    }

    private static void DisplayCaveInstances(PotOverviewViewModel potOverviewViewModel)
    {
        PotsDataGrid potsDataGrid = new()
        {
            Date = potOverviewViewModel.Date,
            Values = potOverviewViewModel.Values,
            Total = potOverviewViewModel.Total
        };

        potsDataGrid.Display();
    }

    private static void DisplayTotals(PotOverviewViewModel potOverviewViewModel)
    {
        Console.WriteLine();

        TotalsDataGrid totalsDataGrid = new()
        {
            Date = potOverviewViewModel.Date,
            Values = potOverviewViewModel.CurrencyTotalOverviews,
            Total = potOverviewViewModel.Total
        };

        totalsDataGrid.Display();
    }

    private static void DisplayConversionRates(PotOverviewViewModel potOverviewViewModel)
    {
        Console.WriteLine();

        ExchangeRatesControl exchangeRatesControl = new()
        {
            ExchangeRates = potOverviewViewModel.ConversionRates
        };

        exchangeRatesControl.Display();
    }
}
