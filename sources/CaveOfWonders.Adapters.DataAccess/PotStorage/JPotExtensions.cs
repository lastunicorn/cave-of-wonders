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

		if (jPot.Snapshots != null)
		{
			IEnumerable<PotSnapshot> potSnapshots = jPot.Snapshots
				.Select(x =>
				{
					PotSnapshot potSnapshot = x.ToPotSnapshot();
					potSnapshot.Pot = pot;

					return potSnapshot;
				});

			pot.Snapshots.AddRange(potSnapshots);
		}

		if (jPot.Labels != null)
			pot.Labels.AddRange(jPot.Labels);

		return pot;
	}
}