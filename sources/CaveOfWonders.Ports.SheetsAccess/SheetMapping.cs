namespace DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

public class SheetMapping
{
    public string Name { get; set; }

    public List<ColumnMappings> ColumnDescriptors { get; set; }
}