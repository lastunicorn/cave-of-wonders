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
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal class GemPersister
{
    private readonly string databaseDirectoryPath;

    public GemPersister(string databaseDirectoryPath)
    {
        this.databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    }

    public async IAsyncEnumerable<Gem> LoadAsync(List<Pot> pots, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        GemsDirectory gemsDirectory = new(databaseDirectoryPath);

        if (!gemsDirectory.Exists)
            yield break;

        IEnumerable<GemsFile> gemsFiles = gemsDirectory.EnumerateGemsFiles();

        foreach (GemsFile gemsFile in gemsFiles)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
            
            List<JGem> jGems = await gemsFile.ReadAsync(cancellationToken);

            foreach (JGem jGem in jGems)
            {
                Gem gem = new()
                {
                    Date = jGem.Date,
                    Amount = jGem.Amount,
                    Description = jGem.Description,
                    Pot = pots?.FirstOrDefault(x => x.Id == gemsFile.PotId)
                };

                if (Enum.TryParse(jGem.Category, out GemCategory category))
                    gem.Category = category;

                foreach (KeyValuePair<string, string> jGemParameter in jGem.Parameters)
                    gem.Parameters.Add(jGemParameter.Key, jGemParameter.Value);
                
                yield return gem;
            }
        }
    }

    public async Task SaveAsync(List<Gem> gems, CancellationToken cancellationToken)
    {
        if (gems == null) throw new ArgumentNullException(nameof(gems));

        GemsDirectory gemsDirectory = new(databaseDirectoryPath);
    
        if (!gemsDirectory.Exists)
            gemsDirectory.Create();

        Dictionary<Guid, List<JGem>> jGemsByPotId = gems
            .Where(x => x.Pot != null)
            .GroupBy(x => x.Pot.Id)
            .ToDictionary(
                x => x.Key,
                x => x
                    .Select(gem =>
                    {
                        JGem jGem = new()
                        {
                            Date = gem.Date,
                            Amount = gem.Amount,
                            Description = gem.Description,
                            Category = gem.Category.ToString()
                        };

                        foreach (KeyValuePair<string, string> gemParameter in gem.Parameters)
                            jGem.Parameters.Add(gemParameter.Key, gemParameter.Value);

                        return jGem;
                    })
                    .ToList());

        foreach (KeyValuePair<Guid, List<JGem>> jGemsByPotIdEntry in jGemsByPotId)
        {
            Guid potId = jGemsByPotIdEntry.Key;
            List<JGem> jGems = jGemsByPotIdEntry.Value;

            GemsFile gemsFile = gemsDirectory.GetGemsFile(potId);
            await gemsFile.SaveAsync(jGems, cancellationToken);
        }
    }
}