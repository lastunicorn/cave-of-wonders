using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

public class ExchangeRateForCurrency
{
    public CurrencyPair CurrencyPair { get; set; }

    public decimal Value { get; set; }
}