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

namespace DustInTheWind.CaveOfWonders.DataTypes;

public record class PotIdentifier
{
    private readonly string partialValue;
    private readonly Guid? guid;

    public bool IsEmpty => !guid.HasValue && partialValue == null;

    public bool IsFullGuid => guid.HasValue;

    public static PotIdentifier Empty { get; } = new PotIdentifier();

    private PotIdentifier()
    {
    }

    public PotIdentifier(string value)
    {
        if (value is not null)
        {
            if (Guid.TryParse(value, out Guid guid))
                this.guid = guid;
            else
                partialValue = value;
        }
    }

    public PotIdentifier(Guid guid)
    {
        this.guid = guid;
    }

    public bool IsMatch(string text)
    {
        if (text == null) return false;

        if (guid.HasValue)
        {
            if (Guid.TryParse(text, out Guid textGuid))
                return guid.Value == textGuid;

            return false;
        }

        if (partialValue is not null)
        {
            int pos = text.IndexOf(partialValue, StringComparison.OrdinalIgnoreCase);
            return pos >= 0;
        }

        return false;
    }

    public override string ToString()
    {
        if (guid.HasValue)
            return guid.Value.ToString("D");

        if (partialValue is not null)
            return partialValue;

        return null;
    }

    public static implicit operator PotIdentifier(string value)
    {
        return new PotIdentifier(value);
    }

    public static implicit operator PotIdentifier(Guid guid)
    {
        return new PotIdentifier(guid);
    }

    public static implicit operator string(PotIdentifier potIdentifier)
    {
        return potIdentifier.ToString();
    }

    public static implicit operator Guid(PotIdentifier potIdentifier)
    {
        if (potIdentifier.guid.HasValue)
            return potIdentifier.guid.Value;

        throw new InvalidCastException("PotIdentifier does not contain a valid Guid.");
    }
}