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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.JsonFileStorage;

internal class ExchangeRatesDirectory
{
    private const string DirectoryName = "Exchange Rates";
    private readonly string directoryPath;

    public ExchangeRatesDirectory(string rootDirectoryPath)
    {
        if (rootDirectoryPath == null) throw new ArgumentNullException(nameof(rootDirectoryPath));
        
        directoryPath = Path.Combine(rootDirectoryPath, DirectoryName);
    }

    public bool Exists => Directory.Exists(directoryPath);

    public IEnumerable<ExchangeRatesFile> EnumerateExchangeRateFiles()
    {
        return Directory.GetFiles(directoryPath)
            .Select(x => new ExchangeRatesFile(x));
    }

    public ExchangeRatesFile GetExchangeRateFile(CurrencyPair currencyPair)
    {
        string fileNameWithoutExtension = currencyPair.ToString().ToLower();
        string fileName = $"{fileNameWithoutExtension}.json";
        string filePath = Path.Combine(directoryPath, fileName);

        return new ExchangeRatesFile(filePath);
    }

    public void Create()
    {
        Directory.CreateDirectory(directoryPath);
    }
}