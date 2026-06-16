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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class PotRepository : IPotRepository
{
    private readonly Database database;

    public PotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<Pot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Pot> result = database.Pots;
        return Task.FromResult(result);
    }

    public Task<Pot> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Pot pot = database.Pots
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(pot);
    }

    public Task<IEnumerable<PotSnapshot>> GetSnapshots(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive)
    {
        IEnumerable<PotSnapshot> potInstances = database.Pots
            .Where(x => includeInactive || x.IsActive(date))
            .Select(x => x.GetSnapshot(date, dateMatchingMode))
            .Where(x => x != null);

        return Task.FromResult(potInstances);
    }

    public async IAsyncEnumerable<Pot> GetByPartialIdAsync(string partialPotId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string idWithoutDashes = partialPotId.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = database.Pots
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase));

        foreach (Pot pot in pots)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return pot;
        }
    }

    public Task<IEnumerable<Pot>> GetByIdOrName(string idOrName)
    {
        string idWithoutDashes = idOrName.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = database.Pots
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase) || (x.Name?.Contains(idOrName, StringComparison.InvariantCultureIgnoreCase) ?? false));

        return Task.FromResult(pots);
    }

    public void Add(Pot pot)
    {
        if (pot == null)
            throw new ArgumentNullException(nameof(pot));

        database.Pots.Add(pot);
    }
}