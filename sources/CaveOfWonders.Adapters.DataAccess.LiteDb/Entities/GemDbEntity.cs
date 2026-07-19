namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;

internal class GemDbEntity
{
	public Guid Id { get; set; }

	public string ExternalId { get; set; }

	public DateTime Date { get; set; }

	public int Category { get; set; }

	public decimal Amount { get; set; }

	public string Description { get; set; }

	public Guid PotId { get; set; }

	public Dictionary<string, string> Parameters { get; set; } = [];
}