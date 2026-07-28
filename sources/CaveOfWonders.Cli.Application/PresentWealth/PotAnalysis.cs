using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal class PotAnalysis
{
	private readonly IUnitOfWork unitOfWork;

	public Pot Pot { get; set; }

	public DateOnly TargetDate { get; set; }

	public SnapshotSelectionMode SnapshotSelectionMode { get; set; }

	public PotAnalysis(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// 1. Select the snapshot
		//		The selected snapshot will provide the base value. Later, gems will ge taken into account. The date of the selected snapshot will be called base date from now on.
		//		if (SnapshotSelectionMode == LastAvailable) => find the last snapshot; if none exist, return null
		//		if (SnapshotSelectionMode == NextAvailable) => find the next snapshot; if none exist, return null
		//		if (SnapshotSelectionMode == Closest) => find the closest snapshot(last or next); if none exist, return null
		//		if (SnapshotSelectionMode == LastAvailableAllowNext) => find the last snapshot; if none exist, find the next snapshot; if none exist, return null
		//		if (SnapshotSelectionMode == NextAvailableAllowLast) => find the next snapshot; if none exist, find the last snapshot; if none exist, return null
		PotSnapshot snapshot = await SelectSnapshot(cancellationToken);

		// 2. Get gems
		//		If base date == null =>
		//			get gems earlier than today
		//			sign = +
		//		If base date < today =>
		//			get all gems from base date to today.
		//			sign = +
		//		If base date > today =>
		//			get all gems from today to base date.
		//			sign = -
		//		If base date == today =>
		//			get no gems
		IEnumerable<Gem> gems = null;
		
		// 3. Calculate value
		//		value = (snapshot?.value ?? 0) + (sign * gem.value)
		//		gem.value is calculated like this:
		//			positive value if gem type == deposit, gain, bonus
		//			negative value if gem type == withdrawal, fee, tax
		//			for unknown gem type => warning
		
		decimal value = snapshot?.Value ?? 0;
		
		foreach (Gem gem in gems)
		{
			// ...
		}
		
		// 4. Calculate normalized value (using currency exchange rates)
		//		Decide the chain of currency exchange rates to use.
		//		TBD
	}

	private async Task<PotSnapshot> SelectSnapshot(CancellationToken cancellationToken)
	{
		return SnapshotSelectionMode switch
		{
			SnapshotSelectionMode.LastAvailable => await GetLastSnapshot(cancellationToken),
			SnapshotSelectionMode.NextAvailable => await GetNextSnapshot(cancellationToken),
			SnapshotSelectionMode.Closest => await GetClosestSnapshot(cancellationToken),
			SnapshotSelectionMode.LastAvailableAllowNext => await GetLastSnapshot(cancellationToken) ?? await GetNextSnapshot(cancellationToken),
			SnapshotSelectionMode.NextAvailableAllowLast => await GetNextSnapshot(cancellationToken) ?? await GetLastSnapshot(cancellationToken),
			_ => throw new ArgumentOutOfRangeException(nameof(SnapshotSelectionMode), SnapshotSelectionMode, null)
		};
	}

	private Task<PotSnapshot> GetLastSnapshot(CancellationToken cancellationToken)
	{
		return unitOfWork.PotSnapshotRepository.GetLastAsync(Pot.Id, TargetDate, cancellationToken);
	}

	private Task<PotSnapshot> GetNextSnapshot(CancellationToken cancellationToken)
	{
		return unitOfWork.PotSnapshotRepository.GetNextAsync(Pot.Id, TargetDate, cancellationToken);
	}

	private async Task<PotSnapshot> GetClosestSnapshot(CancellationToken cancellationToken)
	{
		PotSnapshot lastSnapshot = await GetLastSnapshot(cancellationToken);
		PotSnapshot nextSnapshot = await GetNextSnapshot(cancellationToken);

		if (lastSnapshot == null)
			return nextSnapshot;

		if (nextSnapshot == null)
			return lastSnapshot;

		int daysToLast = TargetDate.DayNumber - lastSnapshot.Date.DayNumber;
		int daysToNext = nextSnapshot.Date.DayNumber - TargetDate.DayNumber;

		return daysToLast <= daysToNext
			? lastSnapshot
			: nextSnapshot;
	}

	// config
	// - SnapshotSelectionMode: [LastAvailable, NextAvailable, Closest, LastAvailableAllowNext, NextAvailableAllowLast]
	// - CurrencySelectionMode: [LastAvailable, NextAvailable, Closest, LastAvailableAllowNext, NextAvailableAllowLast]
}