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

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(databaseDirectoryPath))
            Directory.CreateDirectory(databaseDirectoryPath);

        await LoadExchangeRatesAsync(cancellationToken);
        await LoadPotsAsync(cancellationToken);
        await LoadCpiAsync(cancellationToken);
        await LoadAverageWagesAsync(cancellationToken);
    }

    private async Task LoadExchangeRatesAsync(CancellationToken cancellationToken)
    {
        ExchangeRates.Clear();

        ExchangeRatePersister exchangeRatePersister = new(databaseDirectoryPath);

        await foreach (ExchangeRate exchangeRate in exchangeRatePersister.LoadAsync(cancellationToken))
            ExchangeRates.Add(exchangeRate);
    }

    private async Task LoadPotsAsync(CancellationToken cancellationToken)
    {
        Pots.Clear();

        PotPersister potPersister = new(databaseDirectoryPath);

        await foreach (Pot pot in potPersister.LoadAsync(cancellationToken))
            Pots.Add(pot);
    }

    public async Task LoadGemsAsync(CancellationToken cancellationToken)
    {
        if(areGemsLoaded)
            return;
        
        GemPersister gemPersister = new(databaseDirectoryPath);

        IAsyncEnumerable<Gem> gemCollection = gemPersister.LoadAAsync(cancellationToken);

        List<Gem> temp = Gems.ToList();
        Gems.Clear();
        
        await foreach (Gem gem in gemCollection)
            Gems.Add(gem);
        
        Gems.AddRange(temp);
        
        areGemsLoaded = true;
    }

    private async Task LoadCpiAsync(CancellationToken cancellationToken)
    {
        CpiRecords.Clear();
        CpiPersister cpiPersister = new(databaseDirectoryPath);

        await foreach (Cpi cpi in cpiPersister.LoadAsync(cancellationToken))
            CpiRecords.Add(cpi);
    }

    private async Task LoadAverageWagesAsync(CancellationToken cancellationToken)
    {
        AverageWages.Clear();

        AverageWagePersister averageWagePersister = new(databaseDirectoryPath);

        await foreach (AverageWage averageWage in averageWagePersister.LoadAsync(cancellationToken))
            AverageWages.Add(averageWage);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await SaveExchangeRatesAsync(cancellationToken);
        await SavePotsAsync(cancellationToken);
        await SaveCpiAsync(cancellationToken);
        await SaveAverageWagesAsync(cancellationToken);
        
        if (!areGemsLoaded &&  Gems.Count > 0)
            await LoadGemsAsync(cancellationToken);
        
        if (areGemsLoaded)
            await SaveGemsAsync(cancellationToken);
    }

    private Task SaveExchangeRatesAsync(CancellationToken cancellationToken)
    {
        ExchangeRatePersister exchangeRatePersister = new(databaseDirectoryPath);
        return exchangeRatePersister.SaveAsync(ExchangeRates, cancellationToken);
    }

    private Task SavePotsAsync(CancellationToken cancellationToken)
    {
        PotPersister potPersister = new(databaseDirectoryPath);
        return potPersister.SaveAsync(Pots, cancellationToken);
    }

    private Task SaveCpiAsync(CancellationToken cancellationToken)
    {
        CpiPersister cpiPersister = new(databaseDirectoryPath);
        return cpiPersister.SaveAsync(CpiRecords, cancellationToken);
    }

    private Task SaveAverageWagesAsync(CancellationToken cancellationToken)
    {
        AverageWagePersister averageWagePersister = new(databaseDirectoryPath);
        return averageWagePersister.SaveAsync(AverageWages, cancellationToken);
    }

    private async Task SaveGemsAsync(CancellationToken  cancellationToken)
    {
        GemPersister gemPersister = new(databaseDirectoryPath);
        await gemPersister.SaveAsync(Gems,  cancellationToken);
    }
}