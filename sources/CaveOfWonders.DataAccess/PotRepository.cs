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

    public Task<IEnumerable<PotSnapshot>> GetSnapshot(DateTime date)
    {
        IEnumerable<PotSnapshot> potSnapshots = database.Pots
            .Where(x => x.IsActive(date))
            .Select(x =>
            {
                Gem gem = x.Gems.FirstOrDefault(z => z.Date == date);

                return new PotSnapshot
                {
                    Pot = x,
                    Gem = gem
                };
            });
        return Task.FromResult(potSnapshots);
    }

    public Task<IEnumerable<Pot>> Get(string potName)
    {
        IEnumerable<Pot> pot = database.Pots
            .Where(x => x.Name?.Contains(potName) ?? false);

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
            .Where(x => x.Id.ToString("N").Contains(partialPotId, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(pot);
    }
}