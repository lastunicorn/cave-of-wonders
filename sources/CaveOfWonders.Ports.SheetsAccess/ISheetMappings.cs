namespace DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

public interface ISheetMappings
{
    string Name { get; }

    ColumnMappings[] ColumnDescriptors { get; }
}
