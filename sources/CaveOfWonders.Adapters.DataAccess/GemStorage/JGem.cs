namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal class JGem
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; }
    
    public DateTime Date { get; set; }

    public string Category { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public Dictionary<string, string> Parameters { get; } = [];
}