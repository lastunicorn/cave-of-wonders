using DustInTheWind.Bnr.Toolkit.ExchangeRates;
using System.Globalization;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

internal static class ExchangeRateExtensions
{
    public static IEnumerable<BnrExchangeRate> ToExchangeRates(this Cube cube, Currency referenceCurrency)
    {
        return cube.Rates
            .Select(x => x.ToExchangeRate(cube, referenceCurrency));
    }

    private static BnrExchangeRate ToExchangeRate(this ExchangeRate exchangeRate, Cube cube, Currency referenceCurrency)
    {
        return new BnrExchangeRate
        {
            Date = DateTime.Parse(cube.Date, CultureInfo.InvariantCulture),
            CurrencyPair = (exchangeRate.Currency + referenceCurrency).ToUpper(),
            Value = exchangeRate.Value / exchangeRate.Multiplier
        };
    }
}