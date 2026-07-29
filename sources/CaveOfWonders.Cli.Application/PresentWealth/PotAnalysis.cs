using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;
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
		await Measurement
			.Action($"[{Pot?.Name}] Full analysis", async () =>
			{
				// The value of the pot at a given date is calculated starting from a snapshot (usually the closest snapshot)
				// and adding the gems created from that snapshot until the desired date.

				PotSnapshot snapshot = await RetrieveSnapshot(cancellationToken);
				IAsyncEnumerable<Gem> gems = RetrieveGems(snapshot, cancellationToken);
				Value = await CalculateValue(snapshot, gems, cancellationToken);
				NormalizedValue = await CalculateNormalizedValue(cancellationToken);
			})
			.DisplayToConsole();
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

	private IAsyncEnumerable<Gem> RetrieveGems(PotSnapshot snapshot, CancellationToken cancellationToken)
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
			return AsyncEnumerable.Empty<Gem>();
		}

		return unitOfWork.GemRepository.FindAsync(filter, cancellationToken);
	}

	private async Task<DatedAmount> CalculateValue(PotSnapshot snapshot, IAsyncEnumerable<Gem> gems, CancellationToken cancellationToken)
	{
		return await Measurement
			.Action($"[{Pot?.Name}] Calculate value", async () =>
			{
				int sign = snapshot?.Date > TargetDate
					? -1
					: 1;

				decimal value = snapshot?.Value ?? 0;
				DateOnly? date = snapshot?.Date;
				int daysToTarget = int.MaxValue;

				await foreach (Gem gem in gems.WithCancellation(cancellationToken))
				{
					value += sign * CalculateGemAmount(gem);

					DateOnly gemDate = gem.Date.ToDateOnly();
					int gemDaysToTarget = Math.Abs(gemDate.DayNumber - TargetDate.DayNumber);

					if (date == null || gemDaysToTarget < daysToTarget)
					{
						date = gemDate;
						daysToTarget = gemDaysToTarget;
					}
				}

				return new DatedAmount
				{
					Date = date ?? TargetDate,
					Value = value,
					Currency = Pot.Currency
				};
			})
			.DisplayToConsole()
			.Response();
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

			case GemCategory.Unknown:
			case GemCategory.Internal:
			default:
				log.WriteInfo($"Gem with unknown category '{gem.Category}' was ignored. Pot = '{Pot.Name}' ({Pot.Id:D}); Date = {gem.Date:yyyy-MM-dd}; Amount = {gem.Amount}");
				return 0;
		}
	}

	private async Task<DatedAmount> CalculateNormalizedValue(CancellationToken cancellationToken)
	{
		// Calculate normalized value (using currency exchange rates)
		//		Configuration value needed to control how currency exchange rate is chosen (closest, next, previous, etc.)

		return TargetCurrency != Value.Currency
			? await currencyConverter.Convert(Value, TargetCurrency, TargetDate, cancellationToken)
			: Value;
	}
}