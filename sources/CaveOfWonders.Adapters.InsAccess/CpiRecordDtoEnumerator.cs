using System.Collections;
using System.Globalization;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

internal class CpiRecordDtoEnumerator : IEnumerator<CpiRecordDto>
{
    private readonly CultureInfo cultureInfo = new("ro-RO");
    private int currentLineIndex = -1;
    private readonly IEnumerable<string> lines;
    private IEnumerator<string> lineEnumerator;

    public CpiRecordDto Current { get; private set; }

    object IEnumerator.Current => Current;

    public CpiRecordDtoEnumerator(IEnumerable<string> lines)
    {
        this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
    }

    public bool MoveNext()
    {
        if (lineEnumerator == null)
        {
            IEnumerable<string> filteredLines = lines
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Skip(3);

            lineEnumerator = filteredLines.GetEnumerator();
        }

        while (true)
        {
            bool success = lineEnumerator.MoveNext();

            if (!success)
                return false;

            currentLineIndex++;
            bool recordComplete = ProcessLine();

            if (recordComplete)
                return true;
        }
    }

    public void Reset()
    {
        currentLineIndex = -1;
        Current = null;
        lineEnumerator = null;
    }

    private bool ProcessLine()
    {
        string line = lineEnumerator.Current;

        switch (currentLineIndex % 3)
        {
            case 0:
                int year = int.Parse(line, cultureInfo);

                Current = new CpiRecordDto
                {
                    Year = year
                };

                break;

            case 1:
                decimal value = decimal.Parse(line, cultureInfo);
                Current.Value = value - 100;
                break;

            case 2:
                return true;
        }

        return false;
    }

    public void Dispose()
    {
    }
}