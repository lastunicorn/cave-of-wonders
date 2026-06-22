namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class PotLabelEntity
{
    public int Id { get; set; }

    public Guid PotId { get; set; }

    public PotEntity Pot { get; set; }

    public string Label { get; set; }
}
