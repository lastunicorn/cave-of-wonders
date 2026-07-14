namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal class JPot
{
    public string Name { get; set; }

    public string Description { get; set; }

    public uint DisplayOrder { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Currency { get; set; }
    
    public List<string> Labels { get; set; }

    public List<JSnapshot> Snapshots { get; set; }
}