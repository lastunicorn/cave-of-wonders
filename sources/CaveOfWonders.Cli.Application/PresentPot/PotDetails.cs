using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

public class PotDetails
{
	public Guid Id { get; }

	public string Name { get; }

	public string Description { get; }

	public DateOnly StartDate { get; }

	public DateOnly? EndDate { get; }

	public string Currency { get; }

	public int SnapshotCount { get; }

	public DateOnly? LastSnapshotDate { get; }

	public DatedAmount Value { get; }

	public int GemCount { get; }

	public DateOnly? LatestGemDate { get; }

	public List<string> Labels { get; }

	public bool IsActive { get; }

	internal PotDetails(Pot pot, int gemCount, DateOnly? latestGemDate)
	{
		GemCount = gemCount;
		LatestGemDate = latestGemDate;
		Id = pot.Id;
		Name = pot.Name;
		Description = pot.Description;
		StartDate = pot.StartDate;
		EndDate = pot.EndDate;
		Currency = pot.Currency;
		SnapshotCount = pot.Snapshots.Count;
		Labels = pot.Labels?
			.Select(x => x.Label)
			.ToList() ?? [];

		PotSnapshot lastPotSnapshot = pot.Snapshots?.Count > 0
			? pot.Snapshots[^1]
			: null;

		if (lastPotSnapshot != null)
		{
			LastSnapshotDate = lastPotSnapshot.Date;
			Value = new DatedAmount
			{
				Currency = pot.Currency,
				Value = lastPotSnapshot.Value
			};
		}

		IsActive = pot.EndDate == null || pot.EndDate >= DateOnly.FromDateTime(DateTime.Today);
	}
}