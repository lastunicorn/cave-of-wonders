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

using System.Text.RegularExpressions;

namespace DustInTheWind.CaveOfWonders.Domain;

public struct CurrencyPair : IEquatable<CurrencyPair>
{
    private static readonly Regex Regex = new(@"^(.{3})[\/| ]?(.{3})$", RegexOptions.Singleline);

    public static CurrencyPair Empty = new();

    public CurrencyId Currency1 { get; set; }

    public CurrencyId Currency2 { get; set; }

    public bool IsEmpty => Currency1.IsEmpty || Currency2.IsEmpty;

    public CurrencyPair()
    {
        Currency1 = CurrencyId.Empty;
        Currency2 = CurrencyId.Empty;
    }

    public CurrencyPair(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        Match match = Regex.Match(value);

        if (!match.Success)
            throw new ArgumentException($"Invalid exchange pair identifier: {value}.", nameof(value));

        Currency1 = match.Groups[1].Value;
        Currency2 = match.Groups[2].Value;
    }

    public override string ToString()
    {
        return Currency1 + Currency2;
    }

    public bool Equals(CurrencyPair other)
    {
        return Currency1.Equals(other.Currency1) &&
               Currency2.Equals(other.Currency2);
    }

    public override bool Equals(object obj)
    {
        return obj is CurrencyPair other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Currency1, Currency2);
    }

    public static bool operator ==(CurrencyPair left, CurrencyPair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CurrencyPair left, CurrencyPair right)
    {
        return !left.Equals(right);
    }

    public static implicit operator CurrencyPair(string value)
    {
        return value == null
            ? Empty
            : new CurrencyPair(value);
    }

    public static implicit operator string(CurrencyPair currencyPair)
    {
        return currencyPair.ToString();
    }
}