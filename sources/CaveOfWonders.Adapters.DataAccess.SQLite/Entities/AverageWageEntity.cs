namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class AverageWageEntity
{
    public int Year { get; set; }

    public decimal? GrossValue { get; set; }

    public decimal? NetValue { get; set; }
}
