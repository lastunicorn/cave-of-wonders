namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess;

internal class JPot
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public uint DisplayOrder { get; set; }

    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }

    public string Currency { get; set; }

    public List<JGem> Gems { get; set; }
}