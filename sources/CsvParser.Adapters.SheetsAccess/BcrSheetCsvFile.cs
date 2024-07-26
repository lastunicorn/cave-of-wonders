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

    public IEnumerable<BcrRecord> ParseBcrSheetCsv()
    {
        IEnumerable<string> lines = File.ReadLines(filePath)
            .Skip(1);

        foreach (string line in lines)
        {
            string[] cells = line.Split(';');

            yield return new BcrRecord
            {
                Date = cells[0].ParseDate().Value,
                TotalLei = cells[1].ParseCurrencyLei().Value,
                CurrentAccountLei = cells[2].ParseCurrencyLei(),
                SavingsAccountLei = cells[3].ParseCurrencyLei(),
                DepositAccountLei = cells[4].ParseCurrencyLei(),
                CurrentAccountEuro = cells[5].ParseCurrencyEuro(),
                CurrentAccountEuroConvertedInLei = cells[6].ParseCurrencyEuro()
            };
        }
    }

    public IEnumerable<SheetRecord> Read()
    {
        return Enumerable.Empty<SheetRecord>();
    }
}