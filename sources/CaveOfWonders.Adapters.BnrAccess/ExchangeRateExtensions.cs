using DustInTheWind.Bnr.Toolkit.ExchangeRates;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

internal static class ExchangeRateExtensions
{
    public static IEnumerable<BnrExchangeRate> ToExchangeRates(this DailyExchangeRates dailyExchangeRates, Currency referenceCurrency)
    {
        return dailyExchangeRates.Rates
            .Select(x => x.ToExchangeRate(dailyExchangeRates, referenceCurrency));
    }

    private static BnrExchangeRate ToExchangeRate(this ExchangeRate exchangeRate, DailyExchangeRates dailyExchangeRates, Currency referenceCurrency)
    {
        return new BnrExchangeRate
        {
            Date = dailyExchangeRates.Date,
            CurrencyPair = (exchangeRate.Currency + referenceCurrency).ToUpper(),
            Value = exchangeRate.Value
        };
    }
}