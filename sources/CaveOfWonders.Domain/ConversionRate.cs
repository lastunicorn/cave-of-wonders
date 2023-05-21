namespace DustInTheWind.CaveOfWonders.Domain;

public class ConversionRate
{
    public string SourceCurrency { get; set; }

    public string DestinationCurrency { get; set; }

    public DateTime Date { get; set; }

    public float Value { get; set; }

    public float Convert(float value)
    {
        return value * Value;
    }

    public float ConvertBack(float value)
    {
        return value == 0
            ? 0
            : value / Value;
    }

    public bool CanConvert(string source, string destination)
    {
        return (SourceCurrency == source && DestinationCurrency == destination) ||
               (SourceCurrency == destination && DestinationCurrency == source);
    }
}