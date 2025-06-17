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

namespace DustInTheWind.CaveOfWonders.Infrastructure;

public readonly struct CurrencyId
{
    private readonly string value;

    public static CurrencyId Empty { get; } = new();

    public bool IsEmpty => value == null;

    private CurrencyId(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }

    public static implicit operator CurrencyId(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value.Length != 3) throw new ArgumentException("The currency ID must have three characters.", nameof(value));

        return new CurrencyId(value.ToUpper());
    }

    public static implicit operator string(CurrencyId currencyId)
    {
        return currencyId.value;
    }
}