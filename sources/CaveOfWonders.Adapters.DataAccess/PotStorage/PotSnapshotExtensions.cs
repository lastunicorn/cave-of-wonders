using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal static class PotSnapshotExtensions
{
	public static JSnapshot ToJSnapshot(this PotSnapshot potSnapshot)
	{
		if (potSnapshot == null)
			return null;

		return new JSnapshot
		{
			Date = potSnapshot.Date,
			Value = potSnapshot.Value
		};
	}
}