// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.JsonFileStorage;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

public class Database
{
    private readonly string databaseDirectoryPath;

    public List<Pot> Pots { get; private set; }

    public List<ConversionRate> ConversionRates { get; private set; }

    public Database(string location)
    {
        databaseDirectoryPath = location ?? throw new ArgumentNullException(nameof(location));
    }

    public async Task Load()
    {
        await LoadConversionRates();
        await LoadPots();
    }

    private async Task LoadConversionRates()
    {
        ConversionRatesFile conversionRatesFile = new(databaseDirectoryPath);

        ConversionRates = (await conversionRatesFile.ReadAll())
            .Select(x => new ConversionRate
            {
                SourceCurrency = x.SourceCurrency,
                DestinationCurrency = x.DestinationCurrency,
                Date = x.Date,
                Value = x.Value
            })
            .ToList();
    }

    private async Task LoadPots()
    {
        Pots = new List<Pot>();

        PotsDirectory potsDirectory = new(databaseDirectoryPath);

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

            IEnumerable<Gem> gems = jPot.Gems
                .Select(x => new Gem
                {
                    Date = x.Date,
                    Value = x.Value
                });

            pot.Gems.AddRange(gems);

            Pots.Add(pot);
        }
    }

    public async Task Save()
    {
        await SaveConversionRates();
        await SavePots();
    }

    private Task SaveConversionRates()
    {
        ConversionRatesFile conversionRatesFile = new(databaseDirectoryPath);

        IEnumerable<JConversionRate> conversionRates = ConversionRates
            .Select(x => new JConversionRate
            {
                SourceCurrency = x.SourceCurrency,
                DestinationCurrency = x.DestinationCurrency,
                Date = x.Date,
                Value = x.Value
            });

        return conversionRatesFile.SaveAll(conversionRates);
    }

    private async Task SavePots()
    {
        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        foreach (Pot pot in Pots)
        {
            JPot jPot = new()
            {
                Name = pot.Name,
                Description = pot.Description,
                DisplayOrder = pot.DisplayOrder,
                StartDate = pot.StartDate,
                EndDate = pot.EndDate,
                Currency = pot.Currency,
                Gems = pot.Gems
                    .Select(x => new JGem
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