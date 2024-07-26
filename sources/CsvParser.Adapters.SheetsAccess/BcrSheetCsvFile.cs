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

using DustInTheWind.CsvParser.Ports.SheetsAccess;

namespace DustInTheWind.CsvParser.Adapters.SheetsAccess;

internal class BcrSheetCsvFile
{
    private readonly string filePath;

    public BcrSheetCsvFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public IEnumerable<SheetRecord> Read()
    {
        IEnumerable<string> lines = File.ReadLines(filePath)
            .Skip(1);

        foreach (string line in lines)
        {
            string[] cells = line.Split(';');

            yield return new SheetRecord
            {
                Date = cells[0].ParseDate().Value,
                Values = ParseCells(cells)
                    .ToList()
            };
        }
    }

    private static IEnumerable<SheetValue> ParseCells(IReadOnlyList<string> cells)
    {
        decimal? currentAccountValue = cells[2].ParseCurrencyLei();

        if (currentAccountValue.HasValue)
        {
            yield return new SheetValue
            {
                Name = "current-account",
                Value = currentAccountValue.Value
            };
        }

        decimal? savingsAccountValue = cells[3].ParseCurrencyLei();

        if (savingsAccountValue.HasValue)
        {
            yield return new SheetValue
            {
                Name = "savings-account",
                Value = savingsAccountValue.Value
            };
        }

        decimal? depositAccountValue = cells[4].ParseCurrencyLei();

        if (depositAccountValue.HasValue)
        {
            yield return new SheetValue
            {
                Name = "deposit-account",
                Value = depositAccountValue.Value
            };
        }

        decimal? currentAccountEuroValue = cells[5].ParseCurrencyEuro();

        if (currentAccountEuroValue.HasValue)
        {
            yield return new SheetValue
            {
                Name = "current-account-euro",
                Value = currentAccountEuroValue.Value
            };
        }
    }
}