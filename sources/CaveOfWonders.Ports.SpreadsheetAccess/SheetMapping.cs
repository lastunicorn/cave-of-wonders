namespace DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

public class SheetMapping
{
    public string Name { get; set; }

    public List<ColumnMappings> ColumnDescriptors { get; set; }
}