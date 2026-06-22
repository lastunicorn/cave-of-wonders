namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;

internal class GemParameterEntity
{
    public int Id { get; set; }

    public Guid GemId { get; set; }

    public GemEntity Gem { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }
}
