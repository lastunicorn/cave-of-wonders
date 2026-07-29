namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;

internal class PotSnapshotDbEntity
{
	public DateOnly Date { get; set; }

	public decimal Value { get; set; }

	public bool IsAutomatic { get; set; }
}