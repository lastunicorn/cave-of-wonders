using DustInTheWind.CaveOfWonders.Cli.Application;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal static class CurrencyValueExtensions
{
    public static string ToDisplayString(this CurrencyValue currencyValue)
    {
        if (currencyValue == null)
            return string.Empty;

        return $"{currencyValue.Value:N2} {currencyValue.Currency}";
    }
}