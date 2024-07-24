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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

public class Database
{
    public List<Pot> Pots { get; } = new();

    public List<ConversionRate> ConversionRates { get; } = new()
    {
        new ConversionRate
        {
            SourceCurrency = "EUR",
            DestinationCurrency = "RON",
            Date = new DateTime(2023, 05, 12),
            Value = 4.9294f
        },
        new ConversionRate
        {
            SourceCurrency = "EUR",
            DestinationCurrency = "RON",
            Date = new DateTime(2023, 04, 13),
            Value = 4.9432f
        },
        new ConversionRate
        {
            SourceCurrency = "XAU",
            DestinationCurrency = "RON",
            Date = new DateTime(2023, 05, 12),
            Value = 291.1368f
        },
        new ConversionRate
        {
            SourceCurrency = "XAU",
            DestinationCurrency = "RON",
            Date = new DateTime(2023, 04, 13),
            Value = 292.1414f
        }
    };

    public Database()
    {
        Pots.Add(new Pot
        {
            Name = "BCR Lei (cont curent)",
            StartDate = new DateTime(2015, 06, 01),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 208.75f
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "BCR Lei (cont economii)",
            StartDate = new DateTime(2015, 06, 01),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 30617.02f
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "ING Lei (cont curent)",
            StartDate = new DateTime(2016, 04, 07),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 394.9f
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "ING Lei (cont economii)",
            StartDate = new DateTime(2018, 05, 27),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 25791.54f
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "ING Lei (cont părinți)",
            StartDate = new DateTime(2021, 11, 21),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 10219.66f
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "ING Lei (depozit)",
            StartDate = new DateTime(2023, 02, 27),
            EndDate = new DateTime(2020, 05, 12),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 0
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "Revolut Lei",
            StartDate = new DateTime(2023, 03, 25),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 931.27f
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "Cash Lei",
            StartDate = new DateTime(2015, 06, 01),
            Currency = "RON",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 150
                }
            }
        });

        Pots.Add(new Pot
        {
            Name = "Aur",
            StartDate = new DateTime(2023, 02, 01),
            Currency = "XAU",
            Gems =
            {
                new Gem
                {
                    Date = new DateTime(2023, 05, 12),
                    Value = 100
                },
                new Gem
                {
                    Date = new DateTime(2023, 04, 13),
                    Value = 100
                }
            }
        });
    }
}