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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class GemRepository : IGemRepository
{
    private readonly Database database;

    public GemRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async IAsyncEnumerable<Gem> FindByDateAsync(Guid potId, DateOnly date, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await database.LoadGemsAsync(cancellationToken);

        IEnumerable<Gem> gems = database.Gems
            .Where(x => x.Pot?.Id == potId && DateOnly.FromDateTime(x.Date) == date);

        foreach (Gem gem in gems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return gem;
        }
    }

    public async IAsyncEnumerable<Gem> GetByPotIdAsync(Guid potId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await database.LoadGemsAsync(cancellationToken);

        IEnumerable<Gem> gems = database.Gems
            .Where(x => x.Pot?.Id == potId);

        foreach (Gem gem in gems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return gem;
        }
    }

    public async Task<Gem> GetByDateAsync(Guid potId, DateTime date, CancellationToken cancellationToken = default)
    {
        await database.LoadGemsAsync(cancellationToken);

        return database.Gems
            .FirstOrDefault(x => x.Pot?.Id == potId && x.Date == date);
    }

    public void Add(Gem gem)
    {
        database.Gems.Add(gem);
    }
}