namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

public class GainItem
{
    public string PotName { get; init; }
    
    public string Currency { get; init; }
    
    public CurrencyValue Gain { get; init; }
    
    public CurrencyValue NormalizedGain { get; init; }

    public bool IsActual { get; set; }
}
