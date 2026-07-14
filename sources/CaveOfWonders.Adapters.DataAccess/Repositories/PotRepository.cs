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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Utils;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

public class PotRepository : IPotRepository
{
    private readonly Database database;

    public PotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public IAsyncEnumerable<Pot> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Pot> result = database.Pots;
        return result.ToAsyncEnumerable(cancellationToken);
    }

    public Task<Pot> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Pot pot = database.Pots
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(pot);
    }

    public Task<IEnumerable<PotSnapshot>> GetSnapshotsAsync(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive)
    {
        IEnumerable<PotSnapshot> potInstances = database.Pots
            .Where(x => includeInactive || x.IsActive(date))
            .Select(x => x.GetSnapshot(date, dateMatchingMode))
            .Where(x => x != null);

        return Task.FromResult(potInstances);
    }

    public async IAsyncEnumerable<Pot> GetAsync(PotFlexId potFlexId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IEnumerable<Pot> pots = database.Pots
            .Where(x => potFlexId.IsMatch(x.Id) || potFlexId.IsMatch(x.Name));

        foreach (Pot pot in pots)
            yield return pot;
    }

    public void Add(Pot pot)
    {
        if (pot == null)
            throw new ArgumentNullException(nameof(pot));

        bool alreadyExists = database.Pots.Any(x => x.Id == pot.Id);

        if (alreadyExists)
            throw new ArgumentException($"A pot with id '{pot.Id}' already exists.", nameof(pot));

        database.Pots.Add(pot);
    }
}