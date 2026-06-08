using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

public interface IExcelSpreadsheet : IDisposable
{
    IEnumerable<SheetValue> Read(IEnumerable<SheetMapping> sheetDescriptors);
}