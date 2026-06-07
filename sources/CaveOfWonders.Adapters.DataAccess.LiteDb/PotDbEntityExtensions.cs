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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

internal static class PotDbEntityExtensions
{
    public static Pot ToDomainEntity(this PotDbEntity potDbEntity)
    {
        Pot pot = new()
        {
            Id = potDbEntity.Id,
            Name = potDbEntity.Name,
            Description = potDbEntity.Description,
            DisplayOrder = potDbEntity.DisplayOrder,
            StartDate = potDbEntity.StartDate,
            EndDate = potDbEntity.EndDate,
            Currency = potDbEntity.Currency
        };

        IEnumerable<Gem> gems = potDbEntity.Gems
            .Select(x => new Gem
            {
                Date = x.Date,
                Value = x.Value
            });

        if (potDbEntity.Labels != null)
            pot.Labels.AddRange(potDbEntity.Labels);

        pot.Gems.AddRange(gems);

        return pot;
    }
}
