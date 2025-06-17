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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.JsonFileStorage;

internal class PotFile
{
    private readonly string filePath;

    public Guid PotId { get; }

    public PotFile(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        string idAsString = Path.GetFileNameWithoutExtension(filePath);
        PotId = Guid.Parse(idAsString);
    }

    public async Task<JPot> Read()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonConvert.DeserializeObject<JPot>(json);
    }

    public Task Save(JPot jPot)
    {
        IsoDateTimeConverter dateTimeConverter = new()
        {
            DateTimeFormat = "yyy-MM-dd"
        };
        string json = JsonConvert.SerializeObject(jPot, Formatting.Indented, dateTimeConverter);

        return File.WriteAllTextAsync(filePath, json);
    }
}