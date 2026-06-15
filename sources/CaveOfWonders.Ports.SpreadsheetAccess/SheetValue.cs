namespace DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

public class SheetValue
{
    public Guid Key { get; set; }

    public DateOnly Date { get; set; }

    public decimal Value { get; set; }
}