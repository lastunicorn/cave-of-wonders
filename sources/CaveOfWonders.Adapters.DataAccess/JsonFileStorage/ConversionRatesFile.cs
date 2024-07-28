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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.JsonFileStorage;

internal class ConversionRatesFile
{
    private readonly string rootDirectoryPath;
    private const string FileName = "conversion-rates.json";

    public ConversionRatesFile(string rootDirectoryPath)
    {
        this.rootDirectoryPath = rootDirectoryPath;
    }

    public async Task<List<JConversionRate>> ReadAll()
    {
        string filePath = Path.Combine(rootDirectoryPath, FileName);

        string json = await File.ReadAllTextAsync(filePath);
        return JsonConvert.DeserializeObject<List<JConversionRate>>(json);
    }

    public Task SaveAll(IEnumerable<JConversionRate> conversionRates)
    {
        string filePath = Path.Combine(rootDirectoryPath, FileName);

        IsoDateTimeConverter dateTimeConverter = new()
        {
            DateTimeFormat = "yyy-MM-dd"
        };
        string json = JsonConvert.SerializeObject(conversionRates, Formatting.Indented, dateTimeConverter);

        return File.WriteAllTextAsync(filePath, json);
    }
}