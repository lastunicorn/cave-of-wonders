// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections;
using System.Globalization;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

internal class InflationRecordDtoEnumerator : IEnumerator<InflationRecordDto>
{
    private readonly CultureInfo cultureInfo = new("ro-RO");
    private int currentLineIndex = -1;
    private readonly IEnumerable<string> lines;
    private IEnumerator<string> lineEnumerator;

    public InflationRecordDto Current { get; private set; }

    object IEnumerator.Current => Current;

    public InflationRecordDtoEnumerator(IEnumerable<string> lines)
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

                Current = new InflationRecordDto
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