using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

internal class YearlyInflationDocument
{
    public List<CpiRecordDto> Records { get; } = [];

    public YearlyInflationDocument(IEnumerable<string> lines)
    {
        InflationRecordDtoEnumerator enumerator = new(lines);

        while (enumerator.MoveNext())
            Records.Add(enumerator.Current);
    }
}