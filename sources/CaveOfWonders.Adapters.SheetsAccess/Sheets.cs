using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;
using Newtonsoft.Json;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

public class Sheets : ISheets
{
    public IExcelSpreadsheet GetExcelSpreadsheet(string filePath)
    {
        return new ExcelSpreadsheet(filePath);
    }

    public IEnumerable<SheetMapping> GetMappings(string location)
    {
        string json = File.ReadAllText(location);
        List<JSheetMapping> jSheetDescriptors = JsonConvert.DeserializeObject<List<JSheetMapping>>(json);

        return jSheetDescriptors
            .Select(x => x.ToSheetDescriptor());
    }
}