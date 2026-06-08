using DustInTheWind.CaveOfWonders.Adapters.InsAccess.InflationWebPage;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

/// <summary>
/// INS = Institutul Național de Statistică
/// </summary>
public class Ins : IIns
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
        string url = insConfig.Value.InflationPageUrl;

        if (url == null)
            throw new MissingInflationUrlException();

        InflationWebPageRequest webRequest = new(url);

        InflationWebPageDocument inflationWebPageDocument = await webRequest.Execute();
        return inflationWebPageDocument.EnumerateInflationRecords();
    }
}
