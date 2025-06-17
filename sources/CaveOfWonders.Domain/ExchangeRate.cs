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

using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Domain;

public class ExchangeRate : IEquatable<ExchangeRate>
{
    public DateTime Date { get; set; }

    public CurrencyPair CurrencyPair { get; set; }

    public decimal Value { get; set; }

    public decimal Convert(decimal value)
    {
        return value * Value;
    }

    public decimal ConvertBack(decimal value)
    {
        return value == 0
            ? 0
            : value / Value;
    }

    public ConversionAbility AnalyzeConversionAbility(CurrencyId source, CurrencyId destination)
    {
        bool canConvertDirect = CurrencyPair.Currency1 == source && CurrencyPair.Currency2 == destination;
        if (canConvertDirect)
            return ConversionAbility.ConvertDirect;

        bool canConvertReverse = CurrencyPair.Currency1 == destination && CurrencyPair.Currency2 == source;
        if (canConvertReverse)
            return ConversionAbility.ConvertReverse;

        return ConversionAbility.None;
    }

    public bool CanConvert(CurrencyId source, CurrencyId destination)
    {
        return (CurrencyPair.Currency1 == source && CurrencyPair.Currency2 == destination) ||
               (CurrencyPair.Currency1 == destination && CurrencyPair.Currency2 == source);
    }

    public ExchangeRate Invert()
    {
        return new ExchangeRate
        {
            CurrencyPair = CurrencyPair.Invert(),
            Date = Date,
            Value = ConvertBack(1)
        };
    }

    public bool Equals(ExchangeRate other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Date.Equals(other.Date) && CurrencyPair.Equals(other.CurrencyPair) && Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ExchangeRate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, CurrencyPair, Value);
    }

    public static bool operator ==(ExchangeRate left, ExchangeRate right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ExchangeRate left, ExchangeRate right)
    {
        return !Equals(left, right);
    }
}