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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

internal class CpiPersister
{
    private readonly string databaseDirectoryPath;

    public CpiPersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }

    public async IAsyncEnumerable<Cpi> Load()
    {
        if (!Directory.Exists(databaseDirectoryPath))
            yield break;

        string filePath = Path.Combine(databaseDirectoryPath, "cpi.json");
        CpiFile cpiFile = new(filePath);

        if (!cpiFile.Exists)
            yield break;

        IEnumerable<JCpi> jCpis = await cpiFile.Read();

        IEnumerable<Cpi> cpis = jCpis
            .Select(x => new Cpi
            {
                Year = x.Year,
                Value = x.Value
            });

        foreach (Cpi cpi in cpis)
            yield return cpi;
    }

    public async Task Save(IEnumerable<Cpi> cpis)
    {
        if (!Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        string filePath = Path.Combine(databaseDirectoryPath, "cpi.json");
        CpiFile cpiFile = new(filePath);

        IEnumerable<JCpi> jInflationRecords = cpis
            .Select(x => new JCpi
            {
                Year = x.Year,
                Value = x.Value
            });

        await cpiFile.Save(jInflationRecords);
    }
}