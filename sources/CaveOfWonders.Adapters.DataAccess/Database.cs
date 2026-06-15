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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

public class Database
{
    private readonly string databaseDirectoryPath;

    private bool areGemsLoaded;
    
    public List<Pot> Pots { get; } = [];

    public List<Gem> Gems { get; private set; } = [];
    
    public List<ExchangeRate> ExchangeRates { get; } = [];

    public List<Cpi> CpiRecords { get; } = [];

    public List<AverageWage> AverageWages { get; } = [];

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
        await LoadCpi();
        await LoadAverageWages();
    }

    private async Task LoadExchangeRates()
    {
        ExchangeRates.Clear();

        ExchangeRatePersister exchangeRatePersister = new(databaseDirectoryPath);

        await foreach (ExchangeRate exchangeRate in exchangeRatePersister.Load())
            ExchangeRates.Add(exchangeRate);
    }

    private async Task LoadPots()
    {
        Pots.Clear();

        PotPersister potPersister = new(databaseDirectoryPath);

        await foreach (Pot pot in potPersister.Load())
            Pots.Add(pot);
    }

    public async Task LoadGems(CancellationToken cancellationToken = default)
    {
        if(areGemsLoaded)
            return;
        
        GemPersister gemPersister = new(databaseDirectoryPath);

        IAsyncEnumerable<Gem> gemCollection = gemPersister.Load(cancellationToken);

        List<Gem> temp = Gems.ToList();
        Gems.Clear();
        
        await foreach (Gem gem in gemCollection)
            Gems.Add(gem);
        
        Gems.AddRange(temp);
        
        areGemsLoaded = true;
    }

    private async Task LoadCpi()
    {
        CpiRecords.Clear();
        CpiPersister cpiPersister = new(databaseDirectoryPath);

        await foreach (Cpi cpi in cpiPersister.Load())
            CpiRecords.Add(cpi);
    }

    private async Task LoadAverageWages()
    {
        AverageWages.Clear();

        AverageWagePersister averageWagePersister = new(databaseDirectoryPath);

        await foreach (AverageWage averageWage in averageWagePersister.Load())
            AverageWages.Add(averageWage);
    }

    public async Task Save()
    {
        await SaveExchangeRates();
        await SavePots();
        await SaveCpi();
        await SaveAverageWages();
        
        if (!areGemsLoaded &&  Gems.Count > 0)
            await LoadGems();
        
        if (areGemsLoaded)
            await SaveGems();
    }

    private async Task SaveGems()
    {
        GemPersister gemPersister = new(databaseDirectoryPath);
        await gemPersister.Save(Gems);
    }

    private Task SaveExchangeRates()
    {
        ExchangeRatePersister exchangeRatePersister = new(databaseDirectoryPath);
        return exchangeRatePersister.Save(ExchangeRates);
    }

    private Task SavePots()
    {
        PotPersister potPersister = new(databaseDirectoryPath);
        return potPersister.Save(Pots);
    }

    private Task SaveCpi()
    {
        CpiPersister cpiPersister = new(databaseDirectoryPath);
        return cpiPersister.Save(CpiRecords);
    }

    private Task SaveAverageWages()
    {
        AverageWagePersister averageWagePersister = new(databaseDirectoryPath);
        return averageWagePersister.Save(AverageWages);
    }
}