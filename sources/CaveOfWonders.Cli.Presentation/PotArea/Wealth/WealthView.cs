using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Commando;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Wealth;

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
