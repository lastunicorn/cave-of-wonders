namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public interface IInsService
{
    Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromFile(string filePath);

    Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromWeb();
}