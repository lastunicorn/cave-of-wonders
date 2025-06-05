﻿// Cave of Wonders
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

using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

public class Sheets : ISheets
{
    public IEnumerable<SheetValue> GetBcrRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        BcrSheetCsvFile bcrSheetCsvFile = new(location);
        return bcrSheetCsvFile.Read();
    }

    public IEnumerable<SheetValue> GetIngRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        IngSheetCsvFile ingSheetCsvFile = new(location);
        return ingSheetCsvFile.Read();
    }

    public IEnumerable<SheetValue> GetBrdRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        BrdSheetCsvFile brdSheetCsvFile = new(location);
        return brdSheetCsvFile.Read();
    }

    public IEnumerable<SheetValue> GetBtRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        BtSheetCsvFile btSheetCsvFile = new(location);
        return btSheetCsvFile.Read();
    }

    public IEnumerable<SheetValue> GetRevolutRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        RevolutSheetCsvFile revolutSheetCsvFile = new(location);
        return revolutSheetCsvFile.Read();
    }

    public IEnumerable<SheetValue> GetCashRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        CashSheetCsvFile cashSheetCsvFile = new(location);
        return cashSheetCsvFile.Read();
    }

    public IEnumerable<SheetValue> GetGoldRecords(string location)
    {
        if (location == null) throw new ArgumentNullException(nameof(location));

        GoldSheetCsvFile goldSheetCsvFile = new(location);
        return goldSheetCsvFile.Read();
    }
}