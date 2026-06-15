namespace DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

public interface ISheets
{
    IExcelSpreadsheet GetExcelSpreadsheet(string filePath);

    IEnumerable<SheetMapping> GetMappings(string location);
}