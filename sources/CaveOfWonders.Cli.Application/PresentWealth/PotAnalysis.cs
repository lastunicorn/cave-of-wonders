using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal class PotAnalysis
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ILog log;
	private readonly CurrencyConverter currencyConverter;

	public Pot Pot { get; set; }

	public DateOnly TargetDate { get; set; }

	public Currency TargetCurrency { get; set; }

	public SnapshotSelectionMode SnapshotSelectionMode { get; set; }

	public DatedAmount Value { get; private set; }

	public DatedAmount NormalizedValue { get; private set; }

	public PotAnalysis(IUnitOfWork unitOfWork, ILog log, CurrencyConverter currencyConverter)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.log = log ?? throw new ArgumentNullException(nameof(log));
		this.currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
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
		PotSnapshot snapshot = await RetrieveSnapshot(cancellationToken);

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
		List<Gem> gems = await RetrieveGems(snapshot, cancellationToken);

		// 3. Calculate value
		//		value = (snapshot?.value ?? 0) + (sign * gem.value)
		//		gem.value is calculated like this:
		//			positive value if gem type == deposit, gain, bonus
		//			negative value if gem type == withdrawal, fee, tax
		//			for unknown gem type => warning

		Value = new DatedAmount
		{
			Date = snapshot?.Date ?? TargetDate,
			Value = snapshot?.Value ?? 0,
			Currency = Pot.Currency
		};

		int sign = CalculateSign(snapshot);

		// [bug] The date of the value remains the initial one. It should be changed to be the date of the closest gem to the target date.
		Value += gems.Sum(x => sign * CalculateGemAmount(x));

		// 4. Calculate normalized value (using currency exchange rates)
		//		Configuration value needed to control how currency exchange rate is chosen (closest, next, previous, etc.)
		NormalizedValue = TargetCurrency != Value.Currency
			? await currencyConverter.Convert(Value, TargetCurrency, TargetDate, cancellationToken)
			: Value;
	}

	private async Task<PotSnapshot> RetrieveSnapshot(CancellationToken cancellationToken)
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

	private async Task<List<Gem>> RetrieveGems(PotSnapshot snapshot, CancellationToken cancellationToken)
	{
		DateOnly? baseDate = snapshot?.Date;

		GemFilter filter = new()
		{
			PotId = Pot.Id
		};

		if (baseDate == null)
		{
			filter.EndDate = TargetDate.AddDays(-1);
		}
		else if (baseDate < TargetDate)
		{
			filter.StartDate = baseDate;
			filter.EndDate = TargetDate;
		}
		else if (baseDate > TargetDate)
		{
			filter.StartDate = TargetDate;
			filter.EndDate = baseDate;
		}
		else
		{
			return [];
		}

		return await unitOfWork.GemRepository.FindAsync(filter, cancellationToken)
			.ToListAsync(cancellationToken);
	}

	private int CalculateSign(PotSnapshot snapshot)
	{
		DateOnly? baseDate = snapshot?.Date;

		return baseDate > TargetDate
			? -1
			: 1;
	}

	private decimal CalculateGemAmount(Gem gem)
	{
		switch (gem.Category)
		{
			case GemCategory.Deposit:
			case GemCategory.Gain:
			case GemCategory.Bonus:
				return gem.Amount;

			case GemCategory.Withdrawal:
			case GemCategory.Fee:
			case GemCategory.Tax:
				return -gem.Amount;

			default:
				log.WriteInfo($"Gem with unknown category '{gem.Category}' was ignored. Pot = '{Pot.Name}' ({Pot.Id:D}); Date = {gem.Date:yyyy-MM-dd}; Amount = {gem.Amount}");
				return 0;
		}
	}
}