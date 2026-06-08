using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using HtmlAgilityPack;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess.InflationWebPage;

internal class InflationWebPageDocument
{
    private readonly CultureInfo cultureInfo = new("ro-RO");
    private readonly Stream stream;

    public InflationWebPageDocument(Stream stream)
    {
        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    public IEnumerable<InflationRecordDto> EnumerateInflationRecords()
    {
        HtmlDocument htmlDocument = new();
        htmlDocument.Load(stream);

        HtmlNodeCollection trNodes = htmlDocument.DocumentNode.SelectNodes("//article//table/tbody/tr");

        foreach (HtmlNode trNode in trNodes)
        {
            InflationRecordDto inflationRecordDto = GetInflationRecord(trNode);

            if (inflationRecordDto != null)
                yield return inflationRecordDto;
        }
    }

    private InflationRecordDto GetInflationRecord(HtmlNode trNode)
    {
        HtmlNodeCollection divNodes = trNode.SelectNodes("td/div");

        if (divNodes.Count == 3)
        {
            string yearAsString = divNodes[0].InnerText;
            string valueAsString = divNodes[1].InnerText;

            int year = int.Parse(yearAsString, cultureInfo);
            decimal value = decimal.Parse(valueAsString, cultureInfo) - 100;

            InflationRecordDto inflationRecordDto = new()
            {
                Year = year,
                Value = value
            };

            return inflationRecordDto;
        }

        return null;
    }
}