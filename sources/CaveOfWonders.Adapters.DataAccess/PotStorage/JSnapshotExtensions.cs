using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal static class JSnapshotExtensions
{
	public static PotSnapshot ToPotSnapshot(this JSnapshot jSnapshot)
	{
		if (jSnapshot == null)
			return null;

		return new PotSnapshot
		{
			Date = jSnapshot.Date,
			Value = jSnapshot.Value
		};
	}
}