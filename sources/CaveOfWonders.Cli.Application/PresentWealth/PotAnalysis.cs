using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using System.Diagnostics;

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
				// and adjusting the value by consideting the gems created from that snapshot until the desired date.

				PotSnapshot snapshot = await RetrieveSnapshot(cancellationToken);
				List<Gem> gems = await RetrieveGemsOrderedByDate(snapshot, cancellationToken);
				Value = CalculateValue(snapshot, gems);

				// 4. Calculate normalized value (using currency exchange rates)
				//		Configuration value needed to control how currency exchange rate is chosen (closest, next, previous, etc.)
				NormalizedValue = TargetCurrency != Value.Currency
					? await currencyConverter.Convert(Value, TargetCurrency, TargetDate, cancellationToken)
					: Value;
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

	private async Task<List<Gem>> RetrieveGemsOrderedByDate(PotSnapshot snapshot, CancellationToken cancellationToken)
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
			.OrderBy(x => x.Date)
			.ToListAsync(cancellationToken);
	}

	private DatedAmount CalculateValue(PotSnapshot snapshot, List<Gem> gems)
	{
		return Measurement<DatedAmount>.Action($"[{Pot?.Name}] Calculate value", () =>
			{
				int sign = CalculateSign(snapshot);

				DateOnly? baseDate = gems
					.Select(x => x.Date.ToDateOnly())
					.Concat(snapshot != null
						? new[]
						{
							snapshot.Date
						}
						: Array.Empty<DateOnly>())
					.OrderBy(x => Math.Abs(x.DayNumber - TargetDate.DayNumber))
					.FirstOrDefault();

				decimal value = snapshot?.Value ?? 0;

				value += sign * gems.Sum(CalculateGemAmount);

				return new DatedAmount
				{
					Date = baseDate ?? TargetDate,
					Value = value,
					Currency = Pot.Currency
				};
			})
			.DisplayToConsole()
			.Result;
	}

	// private DatedAmount CalculateValue(PotSnapshot snapshot, List<Gem> gems)
	// {
	// 	int sign = CalculateSign(snapshot);
	//
	// 	DateOnly? baseDate = gems
	// 		.Select(x => x.Date.ToDateOnly())
	// 		.Concat(snapshot != null
	// 			? new[]
	// 			{
	// 				snapshot.Date
	// 			}
	// 			: Array.Empty<DateOnly>())
	// 		.OrderBy(x => Math.Abs(x.DayNumber - TargetDate.DayNumber))
	// 		.FirstOrDefault();
	//
	// 	decimal value = snapshot?.Value ?? 0;
	//
	// 	value += sign * gems.Sum(CalculateGemAmount);
	//
	// 	return new DatedAmount
	// 	{
	// 		Date = baseDate ?? TargetDate,
	// 		Value = value,
	// 		Currency = Pot.Currency
	// 	};
	// }

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

internal static class DateTimeExtensions
{
	public static DateOnly ToDateOnly(this DateTime dateTime)
	{
		return DateOnly.FromDateTime(dateTime);
	}

	public static DateOnly? ToDateOnly(this DateTime? dateTime)
	{
		return dateTime.HasValue
			? DateOnly.FromDateTime(dateTime.Value)
			: null;
	}
}