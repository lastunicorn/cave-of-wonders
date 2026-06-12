using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.Ins.Toolkit;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

/// <summary>
/// INS = Institutul Național de Statistică
/// </summary>
public class InsService : IInsService
{
	private readonly Lazy<InsConfig> insConfig = new(() => new InsConfig());

	public async Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromFile(string filePath)
	{
		string[] lines = await File.ReadAllLinesAsync(filePath);
		YearlyInflationDocument yearlyInflationDocument = new(lines);

		return yearlyInflationDocument.Records;
	}

	public async Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromWeb()
	{
		Uri url = insConfig.Value.InflationPageUrl;

		if (url == null)
			throw new MissingInflationUrlException();

		YearlyInflationWebPage yearlyInflationWebPage = new(url);
		IAsyncEnumerable<YearlyInflationRecord> inflationRecords = yearlyInflationWebPage.EnumerateInflationRecords();

		List<InflationRecordDto> inflationRecordDtos = [];
		await foreach (YearlyInflationRecord inflationRecord in inflationRecords)
		{
			inflationRecordDtos.Add(new InflationRecordDto
			{
				Year = inflationRecord.Year,
				Value = inflationRecord.Value
			});
		}
		return inflationRecordDtos;
	}
}