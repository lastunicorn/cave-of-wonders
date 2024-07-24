using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application;

public class PotInstance
{
    public string Name { get; set; }

    public CurrencyValue OriginalValue { get; set; }

    public CurrencyValue ConvertedValue { get; set; }

    public PotInstance(PotSnapshot potSnapshot)
    {
        Name = potSnapshot.Pot.Name;

        if (potSnapshot.Gem != null)
        {
            OriginalValue = new CurrencyValue
            {
                Currency = potSnapshot.Pot.Currency,
                Value = potSnapshot.Gem.Value
            };
        }
    }
}