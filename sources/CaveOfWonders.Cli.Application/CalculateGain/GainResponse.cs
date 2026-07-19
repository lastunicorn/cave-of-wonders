namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

public class GainResponse
{
    public List<GainItem> Items { get; init; } = [];

    public decimal TotalGain { get; set; }
}