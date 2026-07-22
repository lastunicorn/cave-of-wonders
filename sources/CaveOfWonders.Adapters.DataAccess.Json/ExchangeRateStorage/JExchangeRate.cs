namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.ExchangeRateStorage;

internal class JExchangeRate
{
    public string Currency1 { get; set; }

    public string Currency2 { get; set; }

    public DateOnly Date { get; set; }

    public decimal Value { get; set; }
}