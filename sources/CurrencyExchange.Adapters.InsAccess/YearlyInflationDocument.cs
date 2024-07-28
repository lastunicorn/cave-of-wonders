// Currency Exchange
// Copyright (C) 2023 Dust in the Wind
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

using System.Globalization;
using DustInTheWind.CurrencyExchange.Ports.InsAccess;

namespace DustInTheWind.CurrencyExchange.Adapters.InsAccess;

internal class YearlyInflationDocument
{
    public List<InsInflationRecord> Records { get; } = new();

    public YearlyInflationDocument(IEnumerable<string> lines)
    {
        IEnumerable<string> filteredLines = lines
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Skip(3);

        CultureInfo cultureInfo = new("ro-RO");

        int index = -1;
        InsInflationRecord record = null;

        foreach (string line in filteredLines)
        {
            index++;

            switch (index % 3)
            {
                case 0:
                    if (record != null)
                        Records.Add(record);

                    record = new InsInflationRecord
                    {
                        Year = int.Parse(line, cultureInfo)
                    };

                    break;

                case 1:
                    record.Value = decimal.Parse(line, cultureInfo) - 100;
                    break;
            }
        }

        if (record != null)
            Records.Add(record);
    }
}