using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public interface IInsService
{
	Task<IEnumerable<CpiRecordDto>> GetInflationValuesFromFile(string filePath);

	Task<IEnumerable<CpiRecordDto>> GetInflationValuesFromWeb();

	Task<IEnumerable<AverageWage>> GetAverageWagesAsync();
}