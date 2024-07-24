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
    public List<Pot> Pots { get; private set; }

    public List<ConversionRate> ConversionRates { get; private set; }

    public Database()
    {
        LoadConversionRates();
        LoadPots();

        //ConversionRates = new List<ConversionRate>
        //{
        //    new ConversionRate
        //    {
        //        SourceCurrency = "EUR",
        //        DestinationCurrency = "RON",
        //        Date = new DateTime(2023, 05, 12),
        //        Value = 4.9294f
        //    },
        //    new ConversionRate
        //    {
        //        SourceCurrency = "EUR",
        //        DestinationCurrency = "RON",
        //        Date = new DateTime(2023, 04, 13),
        //        Value = 4.9432f
        //    },
        //    new ConversionRate
        //    {
        //        SourceCurrency = "XAU",
        //        DestinationCurrency = "RON",
        //        Date = new DateTime(2023, 05, 12),
        //        Value = 291.1368f
        //    },
        //    new ConversionRate
        //    {
        //        SourceCurrency = "XAU",
        //        DestinationCurrency = "RON",
        //        Date = new DateTime(2023, 04, 13),
        //        Value = 292.1414f
        //    }
        //};

        //Pots = new List<Pot>();

        //Pots.Add(new Pot
        //{
        //    Name = "BCR Lei (cont curent)",
        //    StartDate = new DateTime(2015, 06, 01),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 208.75
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "BCR Lei (cont economii)",
        //    StartDate = new DateTime(2015, 06, 01),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 30617.02
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "ING Lei (cont curent)",
        //    StartDate = new DateTime(2016, 04, 07),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 394.9
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "ING Lei (cont economii)",
        //    StartDate = new DateTime(2018, 05, 27),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 25791.54
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "ING Lei (cont părinți)",
        //    StartDate = new DateTime(2021, 11, 21),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 10219.66
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "ING Lei (depozit)",
        //    StartDate = new DateTime(2023, 02, 27),
        //    EndDate = new DateTime(2020, 05, 12),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 0
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "Revolut Lei",
        //    StartDate = new DateTime(2023, 03, 25),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 931.27
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "Cash Lei",
        //    StartDate = new DateTime(2015, 06, 01),
        //    Currency = "RON",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 150
        //        }
        //    }
        //});

        //Pots.Add(new Pot
        //{
        //    Name = "Aur",
        //    StartDate = new DateTime(2023, 02, 01),
        //    Currency = "XAU",
        //    Gems =
        //    {
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 05, 12),
        //            Value = 100
        //        },
        //        new Gem
        //        {
        //            Date = new DateTime(2023, 04, 13),
        //            Value = 100
        //        }
        //    }
        //});

        // ----

        //string outputDirectory = @"c:\Projects.pet\CaveOfWonders\db\pots";
        //foreach (Pot pot in Pots)
        //{
        //    IsoDateTimeConverter isoDateTimeConverter = new()
        //    {
        //        DateTimeFormat = "yyyy-MM-dd"
        //    };
        //    string json = JsonConvert.SerializeObject(pot, Formatting.Indented, isoDateTimeConverter);

        //    string fileName = Guid.NewGuid().ToString("D") + ".json";
        //    string filePath = Path.Combine(outputDirectory, fileName);

        //    File.WriteAllText(filePath, json);
        //}

        // ----

        //string outputDirectory = @"c:\Projects.pet\CaveOfWonders\db";

        //IsoDateTimeConverter isoDateTimeConverter = new()
        //{
        //    DateTimeFormat = "yyyy-MM-dd"
        //};
        //string json = JsonConvert.SerializeObject(ConversionRates, Formatting.Indented, isoDateTimeConverter);

        //string fileName = "conversion-rates.json";
        //string filePath = Path.Combine(outputDirectory, fileName);

        //File.WriteAllText(filePath, json);
    }

    private const string DatabaseDirectoryPath = @"c:\Projects.pet\CaveOfWonders\db";

    private void LoadConversionRates()
    {
        string fileName = "conversion-rates.json";
        string filePath = Path.Combine(DatabaseDirectoryPath, fileName);

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
        string directoryName = @"c:\Projects.pet\CaveOfWonders\db\pots";
        string directoryPath = Path.Combine(DatabaseDirectoryPath, directoryName);

        string[] potFilePaths = Directory.GetFiles(directoryPath);

        Pots = new List<Pot>();

        foreach (string potFilePath in potFilePaths)
        {
            string json = File.ReadAllText(potFilePath);
            JPot jPot = JsonConvert.DeserializeObject<JPot>(json);

            Pot pot = new()
            {
                Name = jPot.Name,
                Description = jPot.Description,
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