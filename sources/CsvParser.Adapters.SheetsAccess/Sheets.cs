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

public class Sheets : ISheets
{
    public IEnumerable<SheetRecord> GetRecords(string location, SheetType sheetType)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        switch (sheetType)
        {
            case SheetType.Bcr:
                BcrSheetCsvFile bcrSheetCsvFile = new(location);
                return bcrSheetCsvFile.Read();

            case SheetType.Ing:
                IngSheetCsvFile ingSheetCsvFile = new(location);
                return ingSheetCsvFile.Read();

            case SheetType.Brd:
                break;

            case SheetType.Bt:
                break;

            case SheetType.Revolut:
                break;

            case SheetType.Cash:
                break;

            case SheetType.Gold:
                break;

            case SheetType.TeleTrade:
                break;

            case SheetType.Quanloop:
                break;

            case SheetType.Mintos:
                break;

            case SheetType.PeerBerry:
                break;

            case SheetType.Xtb:
                break;

            case SheetType.Bonds:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(sheetType), sheetType, null);
        }

        return Enumerable.Empty<SheetRecord>();
    }
}