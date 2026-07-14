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

using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

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

    public async Task<Gem> GetByExternalIdAsync(Guid potId, string gemExternalId, CancellationToken cancellationToken)
    {
        await database.LoadGemsAsync(cancellationToken);

        Gem gem = database.Gems
            .FirstOrDefault(x => x.Pot?.Id == potId && x.ExternalId == gemExternalId);

        return gem;
    }

    public async IAsyncEnumerable<Gem> FindByMonthAsync(Guid potId, MonthDate month, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await database.LoadGemsAsync(cancellationToken);

        IEnumerable<Gem> gems = database.Gems
            .Where(x => x.Pot?.Id == potId && x.Date.Month == month.Month && x.Date.Year == month.Year);

        foreach (Gem gem in gems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return gem;
        }
    }

    public async IAsyncEnumerable<Gem> FindByMonthAndCategoryAsync(MonthDate month, GemCategory category, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await database.LoadGemsAsync(cancellationToken);

        IEnumerable<Gem> gems = database.Gems
            .Where(x => x.Date.Year == month.Year && x.Date.Month == month.Month && x.Category == category);

        foreach (Gem gem in gems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return gem;
        }
    }

    public async IAsyncEnumerable<Gem> FindAsync(GemFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await database.LoadGemsAsync(cancellationToken);

        IEnumerable<Gem> gems = database.Gems;

        if (filter.PotId != null)
            gems = gems.Where(x => x.Pot?.Id == filter.PotId.Value);

        if (filter.Date != null)
        {
            gems = gems.Where(x =>
            {
                DateOnly filterDate = filter.Date.Value;
                return x.Date.Year == filterDate.Year && x.Date.Month == filterDate.Month && x.Date.Day == filterDate.Day;
            });
        }

        if (filter.Month != null)
        {
            gems = gems.Where(x =>
            {
                MonthDate filterMonth = filter.Month.Value;
                return x.Date.Year == filterMonth.Year && x.Date.Month == filterMonth.Month;
            });
        }

        if (filter.Categories?.Count > 0)
            gems = gems.Where(x => filter.Categories.Contains(x.Category));

        if (filter.ExternalId != null)
            gems = gems.Where(x => x.ExternalId == filter.ExternalId);

        foreach (Gem gem in gems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return gem;
        }
    }

    public async IAsyncEnumerable<Gem> GetByDateAsync(Guid potId, DateTime date, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await database.LoadGemsAsync(cancellationToken);

        IEnumerable<Gem> gems = database.Gems
            .Where(x => x.Pot?.Id == potId && x.Date == date);

        foreach (Gem gem in gems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return gem;
        }
    }

    public void Add(Gem gem)
    {
        database.Gems.Add(gem);
    }
}