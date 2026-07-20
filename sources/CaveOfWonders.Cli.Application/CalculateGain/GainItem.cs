namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

public class GainItem
{
    public string PotName { get; init; }
    
    public string Currency { get; init; }
    
    public DatedAmount Gain { get; init; }
    
    public DatedAmount NormalizedGain { get; init; }

    public bool IsActual { get; set; }
}
