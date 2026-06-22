namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class PotSnapshotEntity
{
    public int Id { get; set; }

    public Guid PotId { get; set; }

    public PotEntity Pot { get; set; }

    public DateOnly Date { get; set; }

    public decimal Value { get; set; }
}
