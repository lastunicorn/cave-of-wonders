namespace DustInTheWind.CaveOfWonders.Domain;

public record class AverageWage
{
    public int Year { get; set; }
    
    public decimal? GrossValue { get; set; }

    public decimal? NetValue { get; set; }

    public bool IsEmpty => GrossValue is null && NetValue is null;
}