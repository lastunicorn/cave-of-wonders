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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.JsonFileStorage;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class Database
{
    private readonly string databaseDirectoryPath;

    public List<Pot> Pots { get; } = [];

    public List<ExchangeRate> ExchangeRates { get; } = [];

    public List<InflationRecord> InflationRecords { get; } = [];

    public Database(string location)
    {
        databaseDirectoryPath = location ?? throw new ArgumentNullException(nameof(location));
    }

    public async Task Load()
    {
        if (!Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        await LoadExchangeRates();
        await LoadPots();
        await LoadInflationRates();
    }

    private async Task LoadExchangeRates()
    {
        ExchangeRates.Clear();

        ExchangeRatesDirectory exchangeRatesDirectory = new(databaseDirectoryPath);

        if (!exchangeRatesDirectory.Exists)
            return;

        IEnumerable<ExchangeRatesFile> exchangeRateFiles = exchangeRatesDirectory.EnumerateExchangeRateFiles();

        foreach (ExchangeRatesFile exchangeRatesFile in exchangeRateFiles)
        {
            List<JExchangeRate> jExchangeRates = await exchangeRatesFile.ReadAll();

            IEnumerable<ExchangeRate> exchangeRates = jExchangeRates
                .Select(x => new ExchangeRate
                {
                    CurrencyPair = new CurrencyPair(x.Currency1, x.Currency2),
                    Date = x.Date,
                    Value = x.Value
                });

            ExchangeRates.AddRange(exchangeRates);
        }
    }

    private async Task LoadPots()
    {
        Pots.Clear();

        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        if (!potsDirectory.Exists)
            return;

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

            if (jPot.Labels != null)
                pot.Labels.AddRange(jPot.Labels);

            pot.Gems.AddRange(gems);

            Pots.Add(pot);
        }
    }

    private async Task LoadInflationRates()
    {
        string filePath = Path.Combine(databaseDirectoryPath, "inflation-rates.json");
        InflationRatesFile inflationRatesFile = new(filePath);

        if (!inflationRatesFile.Exists)
            return;

        IEnumerable<JInflationRecord> jInflationRecords = await inflationRatesFile.Read();

        IEnumerable<InflationRecord> inflationRecordDtos = jInflationRecords
            .Select(x => new InflationRecord
            {
                Year = x.Year,
                Value = x.Value
            });

        InflationRecords.AddRange(inflationRecordDtos);
    }

    public async Task Save()
    {
        await SaveExchangeRates();
        await SavePots();
        await SaveInflationRates();
    }

    private async Task SaveExchangeRates()
    {
        ExchangeRatesDirectory exchangeRatesDirectory = new(databaseDirectoryPath);

        if (!exchangeRatesDirectory.Exists)
            exchangeRatesDirectory.Create();

        Dictionary<CurrencyPair, IEnumerable<JExchangeRate>> conversionRateGroups = ExchangeRates
            .GroupBy(x => x.CurrencyPair)
            .ToDictionary(x => x.Key, x => x.Select(ToJEntity));

        foreach (KeyValuePair<CurrencyPair, IEnumerable<JExchangeRate>> conversionRateGroup in conversionRateGroups)
        {
            ExchangeRatesFile exchangeRatesFile = exchangeRatesDirectory.GetExchangeRateFile(conversionRateGroup.Key);
            await exchangeRatesFile.SaveAll(conversionRateGroup.Value);
        }
    }

    private static JExchangeRate ToJEntity(ExchangeRate exchangeRate)
    {
        return new JExchangeRate
        {
            Currency1 = exchangeRate.CurrencyPair.Currency1,
            Currency2 = exchangeRate.CurrencyPair.Currency2,
            Date = exchangeRate.Date,
            Value = exchangeRate.Value
        };
    }

    private async Task SavePots()
    {
        PotsDirectory potsDirectory = new(databaseDirectoryPath);

        if (!potsDirectory.Exists)
            potsDirectory.Create();

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
                Labels = pot.Labels?.ToList(),
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

    private async Task SaveInflationRates()
    {
        string filePath = Path.Combine(databaseDirectoryPath, "inflation-rates.json");
        InflationRatesFile inflationRatesFile = new(filePath);

        IEnumerable<JInflationRecord> jInflationRecords = InflationRecords
            .Select(x => new JInflationRecord
            {
                Year = x.Year,
                Value = x.Value
            });

        await inflationRatesFile.Save(jInflationRecords);
    }
}