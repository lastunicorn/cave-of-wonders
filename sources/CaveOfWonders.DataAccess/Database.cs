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

using DustInTheWind.CaveOfWonders.Domain;
using Newtonsoft.Json;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

public class Database
{
    private readonly string databaseDirectoryPath;

    public List<Pot> Pots { get; private set; }

    public List<ConversionRate> ConversionRates { get; private set; }

    public Database(string location)
    {
        databaseDirectoryPath = location ?? throw new ArgumentNullException(nameof(location));

        LoadConversionRates();
        LoadPots();
    }

    private void LoadConversionRates()
    {
        string fileName = "conversion-rates.json";
        string filePath = Path.Combine(databaseDirectoryPath, fileName);

        string json = File.ReadAllText(filePath);
        ConversionRates = JsonConvert.DeserializeObject<List<JConversionRate>>(json)
            .Select(x => new ConversionRate
            {
                SourceCurrency = x.SourceCurrency,
                DestinationCurrency = x.DestinationCurrency,
                Date = x.Date,
                Value = x.Value
            })
            .ToList();
    }

    private void LoadPots()
    {
        string directoryName = "pots";
        string directoryPath = Path.Combine(databaseDirectoryPath, directoryName);

        string[] potFilePaths = Directory.GetFiles(directoryPath);

        Pots = new List<Pot>();

        foreach (string potFilePath in potFilePaths)
        {
            string json = File.ReadAllText(potFilePath);
            JPot jPot = JsonConvert.DeserializeObject<JPot>(json);

            string idAsString = Path.GetFileNameWithoutExtension(potFilePath);
            Guid id = Guid.Parse(idAsString);

            Pot pot = new()
            {
                Id = id,
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
}