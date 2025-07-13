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
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public class PotRepository : IPotRepository
{
    private readonly DbContext dbContext;

    public PotRepository(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<IEnumerable<Pot>> GetAll()
    {
        IEnumerable<Pot> pots = dbContext.Pots
            .FindAll()
            .Select(x => x.ToDomainEntity());

        return Task.FromResult(pots);
    }

    public Task<IEnumerable<PotInstance>> GetInstances(DateTime date, DateMatchingMode dateMatchingMode, bool includeInactive)
    {
        IEnumerable<PotInstance> potInstances = dbContext.Pots
            .FindAll()
            .Select(x => x.ToDomainEntity())
            .Where(x => includeInactive || x.IsActive(date))
            .Select(x => new PotInstance
            {
                Pot = x,
                Gem = x.GetGem(date, dateMatchingMode)
            });

        return Task.FromResult(potInstances);
    }

    public Task<IEnumerable<Pot>> GetByPartialId(string partialPotId)
    {
        string idWithoutDashes = partialPotId.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = dbContext.Pots
            .FindAll()
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.ToDomainEntity());

        return Task.FromResult(pots);
    }

    public Task<IEnumerable<Pot>> GetByIdOrName(string idOrName)
    {
        string idWithoutDashes = idOrName.Trim().Replace("-", string.Empty);

        IEnumerable<Pot> pots = dbContext.Pots
            .FindAll()
            .Where(x => x.Id.ToString("N").Contains(idWithoutDashes, StringComparison.InvariantCultureIgnoreCase) || (x.Name?.Contains(idOrName, StringComparison.InvariantCultureIgnoreCase) ?? false))
            .Select(x => x.ToDomainEntity());

        return Task.FromResult(pots);
    }

    public Task Add(Pot pot)
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
            Gems = pot.Gems
                .Select(x => new GemDbEntity
                {
                    Date = x.Date,
                    Value = x.Value
                })
                .ToList(),
            Labels = pot.Labels?.ToList() ?? []
        };

        dbContext.Pots.Insert(potDbEntity);

        return Task.CompletedTask;
    }
}
