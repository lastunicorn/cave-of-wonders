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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.JsonFileStorage;

internal class AverageWagesFile
{
    private readonly string filePath;

    public bool Exists => File.Exists(filePath);

    public AverageWagesFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task<IEnumerable<JAverageWageRecord>> Read()
    {
        if (!File.Exists(filePath))
            return Enumerable.Empty<JAverageWageRecord>();

        string json = await File.ReadAllTextAsync(filePath);

        return string.IsNullOrEmpty(json)
            ? Enumerable.Empty<JAverageWageRecord>()
            : JsonConvert.DeserializeObject<IEnumerable<JAverageWageRecord>>(json);
    }

    public Task Save(IEnumerable<JAverageWageRecord> jAverageWageRecords)
    {
        string json = JsonConvert.SerializeObject(jAverageWageRecords, Formatting.Indented);
        return File.WriteAllTextAsync(filePath, json);
    }
}