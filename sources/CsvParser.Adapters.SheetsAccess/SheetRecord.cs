// Cave of Wonders
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
using DustInTheWind.CsvParser.Ports.SheetsAccess;

namespace DustInTheWind.CsvParser.Adapters.SheetsAccess;

internal class SheetRecord
{
    private readonly IEnumerable<ColumnDescriptor> columnDescriptors;
    private readonly string[] cells;

    public SheetRecord(string line, IEnumerable<ColumnDescriptor> columnDescriptors)
    {
        this.columnDescriptors = columnDescriptors ?? throw new ArgumentNullException(nameof(columnDescriptors));
        cells = line.Split(';');
    }

    public IEnumerable<SheetValue> ParseCells()
    {
        return columnDescriptors
            .Select(ParseCell)
            .Where(x => x != null);
    }

    private SheetValue ParseCell(ColumnDescriptor columnDescriptor)
    {
        decimal? value = columnDescriptor.Format switch
        {
            ValueFormat.Lei => ParseCellLei(columnDescriptor.Index),
            ValueFormat.Euro => ParseCellEuro(columnDescriptor.Index),
            _ => throw new ArgumentOutOfRangeException()
        };

        return value.HasValue
            ? new SheetValue
            {
                Key = columnDescriptor.Key,
                Date = ParseDate(columnDescriptor.DateIndex),
                Value = value.Value
            }
            : null;
    }

    private decimal? ParseCellLei(int cellIndex)
    {
        string value = cells[cellIndex];

        if (string.IsNullOrEmpty(value))
            return null;

        return decimal.Parse(value, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("ro-RO"));
    }

    private decimal? ParseCellEuro(int cellIndex)
    {
        string value = cells[cellIndex];

        if (string.IsNullOrEmpty(value))
            return null;

        return decimal.Parse(value.Trim('€'), NumberStyles.Currency, CultureInfo.CreateSpecificCulture("ro-RO"));
    }

    private DateTime ParseDate(int cellIndex)
    {
        string value = cells[cellIndex];
        return DateTime.Parse(value);
    }
}