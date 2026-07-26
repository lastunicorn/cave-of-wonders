using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal static class PotExtensions
{
	public static JPot ToJPot(this Pot pot, IEnumerable<PotSnapshot> snapshots)
	{
		if (pot == null)
			return null;

		return new JPot
		{
			Name = pot.Name,
			Description = pot.Description,
			DisplayOrder = pot.DisplayOrder,
			StartDate = pot.StartDate,
			EndDate = pot.EndDate,
			Currency = pot.Currency,
			Labels = pot.Labels?
				.Select(x => x.Label)
				.ToList(),
			Snapshots = snapshots
				.Select(x => x.ToJSnapshot())
				.ToList()
		};
	}
}