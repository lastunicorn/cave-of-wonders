﻿// Cave of Wonders
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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class PotRepository : IPotRepository
{
    private readonly Database database;

    public PotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<Pot>> GetAll()
    {
        IEnumerable<Pot> result = database.Pots;
        return Task.FromResult(result);
    }

    public Task<IEnumerable<PotInstance>> GetInstances(DateTime date, DateMatchingMode dateMatchingMode, bool includeInactive)
    {
        IEnumerable<PotInstance> potSnapshots = database.Pots
            .Where(x => includeInactive || x.IsActive(date))
            .Select(x => new PotInstance
            {
                Pot = x,
                Gem = x.GetGem(date, dateMatchingMode)
            });

        return Task.FromResult(potSnapshots);
    }

    public Task<IEnumerable<Pot>> GetByName(string potName)
    {
        IEnumerable<Pot> pot = database.Pots
            .Where(x => x.Name?.Contains(potName, StringComparison.InvariantCultureIgnoreCase) ?? false);

        return Task.FromResult(pot);
    }

    public Task<Pot> GetById(Guid potId)
    {
        Pot pot = database.Pots.FirstOrDefault(x => x.Id == potId);
        return Task.FromResult(pot);
    }

    public Task<IEnumerable<Pot>> GetByPartialId(string partialPotId)
    {
        IEnumerable<Pot> pot = database.Pots
            .Where(x => x.Id.ToString("D").Contains(partialPotId, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(pot);
    }

    public Task<IEnumerable<Pot>> GetByIdOrName(string idOrName)
    {
        IEnumerable<Pot> pots = database.Pots
            .Where(x => x.Id.ToString("D").Contains(idOrName, StringComparison.InvariantCultureIgnoreCase) || (x.Name?.Contains(idOrName, StringComparison.InvariantCultureIgnoreCase) ?? false));

        return Task.FromResult(pots);
    }
}