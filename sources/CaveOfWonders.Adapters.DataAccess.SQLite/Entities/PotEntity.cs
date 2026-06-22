namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class PotEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public uint DisplayOrder { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Currency { get; set; }

    public List<PotSnapshotEntity> Snapshots { get; set; } = [];

    public List<PotLabelEntity> Labels { get; set; } = [];
}
