namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class GemEntity
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; }

    public DateTime Date { get; set; }

    public int Category { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public Guid PotId { get; set; }

    public PotEntity Pot { get; set; }

    public List<GemParameterEntity> Parameters { get; set; } = [];
}
