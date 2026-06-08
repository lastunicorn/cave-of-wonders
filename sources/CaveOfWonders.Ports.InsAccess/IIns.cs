namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public interface IIns
{
    Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromFile(string filePath);

    Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromWeb();
}