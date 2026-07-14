namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.AverageWageStorage;

internal class JAverageWageRecord
{
    public int Year { get; set; }

    public decimal? Gross { get; set; }
    
    public decimal? Net { get; set; }
}