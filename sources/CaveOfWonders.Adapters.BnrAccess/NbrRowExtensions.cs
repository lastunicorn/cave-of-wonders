using System.Globalization;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

internal static class NbrRowExtensions
{
    public static IEnumerable<BnrExchangeRate> ToExchangeRates(this NbrCube cube, string origCurrency)
    {
        return cube.Rates
            .Where(x => x.Value != null && x.Value != "-")
            .Select(x => x.ToExchangeRate(cube, origCurrency));
    }

    private static BnrExchangeRate ToExchangeRate(this NbrRate nbrRate, NbrCube cube, string origCurrency)
    {
        int multiplier = 1;

        if (nbrRate.Multiplier != null)
            multiplier = int.Parse(nbrRate.Multiplier);

        return new BnrExchangeRate
        {
            Date = DateTime.Parse(cube.Date, CultureInfo.InvariantCulture),
            CurrencyPair = (nbrRate.Currency + origCurrency).ToUpper(),
            Value = decimal.Parse(nbrRate.Value, CultureInfo.InvariantCulture) / multiplier
        };
    }
}