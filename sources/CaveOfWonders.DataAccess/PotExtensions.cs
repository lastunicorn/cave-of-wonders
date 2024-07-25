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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

internal static class PotExtensions
{
    public static Gem GetGem(this Pot pot, DateTime date, DateMatchingMode dateMatchingMode)
    {
        return dateMatchingMode switch
        {
            DateMatchingMode.Exact => pot.Gems.FirstOrDefault(z => z.Date == date),
            DateMatchingMode.LastAvailable => pot.Gems
                .Where(z => z.Date <= date)
                .MaxBy(z => z.Date),
            _ => throw new ArgumentOutOfRangeException(nameof(dateMatchingMode))
        };
    }
}