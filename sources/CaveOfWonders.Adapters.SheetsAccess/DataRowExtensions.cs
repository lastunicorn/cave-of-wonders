// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

using System.Data;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

internal static class DataRowExtensions
{
    public static DateTime GetDate(this DataRow row, int index)
    {
        object value = row[index];
        return value == DBNull.Value
            ? DateTime.MinValue
            : Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }

    public static float GetFloat(this DataRow row, int index)
    {
        object value = row[index];
        return value == DBNull.Value
            ? 0f
            : Convert.ToSingle(value, CultureInfo.InvariantCulture);
    }

    public static decimal GetDecimal(this DataRow row, int index)
    {
        object value = row[index];
        return value == DBNull.Value
            ? 0m
            : Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }
}