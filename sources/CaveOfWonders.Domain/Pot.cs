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

namespace DustInTheWind.CaveOfWonders.Domain;

public class Pot
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public uint DisplayOrder { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Currency { get; set; }

    public List<Gem> Gems { get; } = [];
    
    public List<string> Labels { get; } = [];

    public bool IsActive(DateTime date)
    {
        return date >= StartDate && (EndDate == null || date <= EndDate);
    }

    public Gem GetLastGem()
    {
        bool hasGems = Gems.Count > 0;

        return hasGems
            ? Gems[Gems.Count - 1]
            : null;
    }
}