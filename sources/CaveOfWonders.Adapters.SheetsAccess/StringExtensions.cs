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

using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

internal static class StringExtensions
{
    public static decimal? ParseCurrencyLei(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return decimal.Parse(value, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("ro-RO"));
    }

    public static decimal? ParseCurrencyEuro(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return decimal.Parse(value.Trim('€'), NumberStyles.Currency, CultureInfo.CreateSpecificCulture("ro-RO"));
    }

    public static DateTime? ParseDate(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return DateTime.Parse(value);
    }

    public static string ToSafeString(this DateTime? dateTime)
    {
        if (dateTime == null)
            return "{null}";

        return dateTime.Value.ToString("yyyy-MM-dd");
    }

    public static string ToSafeString(this decimal? value)
    {
        if (value == null)
            return "{null}";

        return value.Value.ToString(CultureInfo.InvariantCulture);
    }
}