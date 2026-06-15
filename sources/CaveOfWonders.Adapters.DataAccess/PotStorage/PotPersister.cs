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

internal class PotPersister
{
    private readonly string databaseDirectoryPath;

    public PotPersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }
    
    public async IAsyncEnumerable<Pot> Load()
    {
        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        if (!potsDirectory.Exists)
            yield break;

        IEnumerable<PotFile> potFiles = potsDirectory.EnumeratePotFiles();

        foreach (PotFile potFile in potFiles)
        {
            JPot jPot = await potFile.Read();

            Pot pot = new()
            {
                Id = potFile.PotId,
                Name = jPot.Name,
                Description = jPot.Description,
                DisplayOrder = jPot.DisplayOrder,
                StartDate = jPot.StartDate,
                EndDate = jPot.EndDate,
                Currency = jPot.Currency
            };

            IEnumerable<PotSnapshot> potSnapshots = jPot.Snapshots
                .Select(x => new PotSnapshot
                {
                    Date = x.Date,
                    Value = x.Value,
                    Pot = pot
                });

            if (jPot.Labels != null)
                pot.Labels.AddRange(jPot.Labels);

            pot.Snapshots.AddRange(potSnapshots);

            yield return pot;
        }
    }

    public async Task Save(IEnumerable<Pot> pots)
    {
        if (pots == null) throw new ArgumentNullException(nameof(pots));
        
        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        if (!potsDirectory.Exists)
            potsDirectory.Create();

        foreach (Pot pot in pots)
        {
            JPot jPot = new()
            {
                Name = pot.Name,
                Description = pot.Description,
                DisplayOrder = pot.DisplayOrder,
                StartDate = pot.StartDate,
                EndDate = pot.EndDate,
                Currency = pot.Currency,
                Labels = pot.Labels?.ToList(),
                Snapshots = pot.Snapshots
                    .Select(x => new JSnapshot
                    {
                        Date = x.Date,
                        Value = x.Value
                    })
                    .ToList()
            };

            PotFile potFile = potsDirectory.GetPotFile(pot.Id);
            await potFile.Save(jPot);
        }
    }
}