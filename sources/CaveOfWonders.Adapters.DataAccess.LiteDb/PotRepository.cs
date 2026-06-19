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
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Utils;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class PotRepository : IPotRepository
{
    private readonly DbContext dbContext;

    public PotRepository(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<Pot> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Pot> pots = dbContext.Pots
            .FindAll()
            .Select(x => x.ToDomainEntity());

        return pots.ToAsyncEnumerable(cancellationToken);
    }

    public Task<Pot> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PotSnapshot>> GetSnapshots(DateOnly date, DateMatchingMode dateMatchingMode, bool includeInactive)
    {
        IEnumerable<PotSnapshot> potInstances = dbContext.Pots
            .FindAll()
            .Select(x => x.ToDomainEntity())
            .Where(x => includeInactive || x.IsActive(date))
            .Select(x => x.GetSnapshot(date, dateMatchingMode));

        return Task.FromResult(potInstances);
    }

    public IAsyncEnumerable<Pot> GetByPartialIdAsync(string partialPotId, CancellationToken cancellationToken = default)
    {
        string idWithoutDashes = partialPotId.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = dbContext.Pots
            .FindAll()
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.ToDomainEntity());

        return pots.ToAsyncEnumerable(cancellationToken);
    }

    public IAsyncEnumerable<Pot> GetByIdOrName(string idOrName, CancellationToken cancellationToken)
    {
        string idWithoutDashes = idOrName.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = dbContext.Pots
            .FindAll()
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase) || (x.Name?.Contains(idOrName, StringComparison.InvariantCultureIgnoreCase) ?? false))
            .Select(x => x.ToDomainEntity());

        return pots.ToAsyncEnumerable(cancellationToken);
    }

    public void Add(Pot pot)
    {
        ArgumentNullException.ThrowIfNull(pot);

        PotDbEntity potDbEntity = new()
        {
            Id = pot.Id,
            Name = pot.Name,
            Description = pot.Description,
            DisplayOrder = pot.DisplayOrder,
            StartDate = pot.StartDate,
            EndDate = pot.EndDate,
            Currency = pot.Currency,
            Snapshots = pot.Snapshots
                .Select(x => new PotSnapshotDbEntity
                {
                    Date = x.Date,
                    Value = x.Value
                })
                .ToList(),
            Labels = pot.Labels?.ToList() ?? []
        };

        dbContext.Pots.Insert(potDbEntity);
    }
}