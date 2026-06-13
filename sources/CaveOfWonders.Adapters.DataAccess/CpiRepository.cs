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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class CpiRepository : ICpiRepository
{
    private readonly Database database;

    public CpiRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<IEnumerable<Cpi>> GetAll()
    {
        IEnumerable<Cpi> inflationRecords = database.CpiRecords;
        return Task.FromResult(inflationRecords);
    }

    public Task Add(Cpi cpiDto)
    {
        if (cpiDto == null) throw new ArgumentNullException(nameof(cpiDto));

        database.CpiRecords.Add(cpiDto);

        return Task.CompletedTask;
    }

    public Task<AddOrUpdateResult> AddOrUpdate(Cpi cpiDto)
    {
        if (cpiDto == null) throw new ArgumentNullException(nameof(cpiDto));

        Cpi existing = database.CpiRecords
            .FirstOrDefault(x => x.Year == cpiDto.Year);

        if (existing == null)
        {
            database.CpiRecords.Add(cpiDto);
            return Task.FromResult(AddOrUpdateResult.Added);
        }
        else
        {
            existing.Value = cpiDto.Value;
            return Task.FromResult(AddOrUpdateResult.Updated);
        }
    }
}