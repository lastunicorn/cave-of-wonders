namespace DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

public interface IExcelSpreadsheet : IDisposable
{
    IEnumerable<SheetValue> Read(IEnumerable<SheetMapping> sheetDescriptors);
}