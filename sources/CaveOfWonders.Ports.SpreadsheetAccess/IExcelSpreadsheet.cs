namespace DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

public interface IExcelSpreadsheet : IDisposable
{
	IEnumerable<SheetValue> Read(IEnumerable<SheetMapping> sheetDescriptors);
}