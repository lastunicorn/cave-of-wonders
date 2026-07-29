using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

internal static class PotDbEntityExtensions
{
	public static Pot ToDomainEntity(this PotDbEntity potDbEntity)
	{
		Pot pot = new()
		{
			Id = potDbEntity.Id,
			Name = potDbEntity.Name,
			Description = potDbEntity.Description,
			DisplayOrder = potDbEntity.DisplayOrder,
			StartDate = potDbEntity.StartDate,
			EndDate = potDbEntity.EndDate,
			Currency = potDbEntity.Currency
		};

		if (potDbEntity.Labels != null)
		{
			pot.Labels.AddRange(potDbEntity.Labels
				.Select(x => new PotLabel
				{
					Label = x
				}));
		}

		return pot;
	}

	public static List<PotSnapshot> ToPotSnapshots(this PotDbEntity potDbEntity, Pot pot)
	{
		return potDbEntity.Snapshots
			.Select(x => new PotSnapshot
			{
				Date = x.Date,
				Value = x.Value,
				IsAutomatic = x.IsAutomatic,
				Pot = pot
			})
			.ToList();
	}
}