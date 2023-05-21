using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

public class PotInstance
{
    public string Name { get; set; }

    public CurrencyValue Value { get; set; }

    public CurrencyValue ConvertedValue { get; set; }

    public PotInstance(PotSnapshot potSnapshot)
    {
        Name = potSnapshot.Pot.Name;

        if (potSnapshot.Gem != null)
        {
            Value = new CurrencyValue
            {
                Currency = potSnapshot.Pot.Currency,
                Value = potSnapshot.Gem.Value
            };
        }
    }
}