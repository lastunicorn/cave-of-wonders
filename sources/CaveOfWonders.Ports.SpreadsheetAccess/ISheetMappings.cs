namespace DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

public interface ISheetMappings
{
    string Name { get; }

    ColumnMappings[] ColumnDescriptors { get; }
}
