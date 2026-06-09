namespace DustInTheWind.CaveOfWonders.Ports.BnrAccess;

public class BnrExchangeRate
{
    public DateOnly Date { get; set; }

    public string CurrencyPair { get; set; }

    public decimal Value { get; set; }
}