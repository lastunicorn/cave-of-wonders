using DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

namespace DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

public interface ISheets
{
    IExcelSpreadsheet GetExcelSpreadsheet(string filePath);

    IEnumerable<SheetMapping> GetMappings(string location);
}