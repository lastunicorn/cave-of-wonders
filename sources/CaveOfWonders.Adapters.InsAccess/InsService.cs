using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.Ins.Toolkit;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

/// <summary>
/// INS = Institutul Național de Statistică
/// </summary>
public class InsService : IInsService
{
	private readonly Lazy<InsConfig> insConfig = new(() => new InsConfig());

	public async Task<IEnumerable<CpiRecordDto>> GetInflationValuesFromFile(string filePath)
	{
		string[] lines = await File.ReadAllLinesAsync(filePath);
		YearlyInflationDocument yearlyInflationDocument = new(lines);

		return yearlyInflationDocument.Records;
	}

	public async Task<IEnumerable<CpiRecordDto>> GetInflationValuesFromWeb()
	{
		Uri url = insConfig.Value.CpiPageUrl;

		if (url == null)
			throw new MissingCpiUrlException();

		YearlyCpiWebPage yearlyCpiWebPage = new(url);
		IEnumerable<YearlyCpiRecord> yearlyCpiRecords = await yearlyCpiWebPage.EnumerateRecords();

		return yearlyCpiRecords
			.Select(x => new CpiRecordDto
			{
				Year = x.Year,
				Value = x.Value
			});
	}

	public async Task<IEnumerable<AverageWage>> GetAverageWagesAsync()
	{
		YearlyAverageWageWebPage webPage = new();

		return (await webPage.EnumerateRecords())
			.Select(x => new AverageWage
			{
				Year = x.Year,
				GrossValue = x.AverageGrossWage,
				NetValue = x.AverageNetWage
			});
	}
}