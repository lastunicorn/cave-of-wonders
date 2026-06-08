namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public class DuplicateReport
{
    public DateTime Date { get; set; }

    public string CurrencyPair { get; set; }

    public decimal Value1 { get; set; }

    public decimal Value2 { get; set; }
}