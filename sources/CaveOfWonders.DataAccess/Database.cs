using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.DataAccess;

internal static class Database
{
    public static List<Pot> Pots { get; } = new();

    static Database()
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