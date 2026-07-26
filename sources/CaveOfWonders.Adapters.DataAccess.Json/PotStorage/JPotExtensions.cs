using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal static class JPotExtensions
{
	public static Pot ToPot(this JPot jPot)
	{
		if (jPot == null) throw new ArgumentNullException(nameof(jPot));

		Pot pot = new()
		{
			Name = jPot.Name,
			Description = jPot.Description,
			DisplayOrder = jPot.DisplayOrder,
			StartDate = jPot.StartDate,
			EndDate = jPot.EndDate,
			Currency = jPot.Currency
		};

		if (jPot.Labels != null)
			pot.Labels.AddRange(jPot.Labels
				.Select(x => new PotLabel
				{
					Label = x
				}));

		return pot;
	}

	public static List<PotSnapshot> ToPotSnapshots(this JPot jPot, Pot pot)
	{
		if (jPot == null) throw new ArgumentNullException(nameof(jPot));

		if (jPot.Snapshots == null)
			return [];

		return jPot.Snapshots
			.Select(x =>
			{
				PotSnapshot potSnapshot = x.ToPotSnapshot();
				potSnapshot.Pot = pot;

				return potSnapshot;
			})
			.ToList();
	}
}