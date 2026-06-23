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

public class AverageWageRepository : IAverageWageRepository
{
    private readonly Database database;

    public AverageWageRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<AverageWage> GetAsync(int averageWageYear, CancellationToken cancellationToken)
    {
        AverageWage averageWage = database.AverageWages
            .FirstOrDefault(x => x.Year == averageWageYear);

        return Task.FromResult(averageWage);
    }

    public async IAsyncEnumerable<AverageWage> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IEnumerable<AverageWage> averageWages = database.AverageWages;

        foreach (AverageWage averageWage in averageWages)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return averageWage;
        }
    }

    public void Add(AverageWage averageWage)
    {
        if (averageWage == null)
            throw new ArgumentNullException(nameof(averageWage));

        database.AverageWages.Add(averageWage);
    }

    public void Delete(AverageWage averageWage)
    {
        if (averageWage == null)
            throw new ArgumentNullException(nameof(averageWage));

        database.AverageWages.Remove(averageWage);
    }
}