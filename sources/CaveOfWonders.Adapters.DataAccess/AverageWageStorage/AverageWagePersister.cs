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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.JsonFileStorage;
using DustInTheWind.CaveOfWonders.Domain;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

internal class AverageWagePersister
{
    private readonly string databaseDirectoryPath;

    public AverageWagePersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }

    public async IAsyncEnumerable<AverageWage> LoadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!Directory.Exists(databaseDirectoryPath))
            yield break;

        string filePath = Path.Combine(databaseDirectoryPath, "average-wages.json");
        AverageWagesFile averageWagesFile = new(filePath);

        if (!averageWagesFile.Exists)
            yield break;

        IEnumerable<JAverageWageRecord> jAverageWageRecords = await averageWagesFile.ReadAsync(cancellationToken);

        IEnumerable<AverageWage> averageWageRecordDtos = jAverageWageRecords
            .Select(x => new AverageWage { Year = x.Year, GrossValue = x.Gross, NetValue = x.Net });

        foreach (AverageWage averageWage in averageWageRecordDtos)
            yield return averageWage;
    }

    public async Task SaveAsync(IEnumerable<AverageWage> averageWages, CancellationToken cancellationToken)
    {
        if (averageWages == null) throw new ArgumentNullException(nameof(averageWages));
        
        if (!Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        string filePath = Path.Combine(databaseDirectoryPath, "average-wages.json");
        AverageWagesFile averageWageFile = new(filePath);

        IEnumerable<JAverageWageRecord> jAverageWageRecord = averageWages
            .Select(x => new JAverageWageRecord { Year = x.Year, Gross = x.GrossValue, Net = x.NetValue });

        await averageWageFile.SaveAsync(jAverageWageRecord, cancellationToken);
    }
}