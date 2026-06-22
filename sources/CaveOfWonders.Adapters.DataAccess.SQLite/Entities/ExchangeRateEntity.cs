namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class ExchangeRateEntity
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string CurrencyPair { get; set; }

    public decimal Value { get; set; }
}
