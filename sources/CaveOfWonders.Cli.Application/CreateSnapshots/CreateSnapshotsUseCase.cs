using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CreateSnapshots;

internal class CreateSnapshotsUseCase : IUseCase<CreateSnapshotsRequest, CreateSnapshotsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly OperationManager operationManager;

	public CreateSnapshotsUseCase(IUnitOfWork unitOfWork, OperationManager operationManager)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<CreateSnapshotsResponse> Execute(CreateSnapshotsRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await operationManager.CreateAndExecuteAsync<GetOnePotOperation, Pot>(
			op =>
			{
				op.PotId = request.PotId;
			},
			cancellationToken);

		CreateSnapshotsResponse response = new()
		{
			PotId = pot.Id,
			PotName = pot.Name
		};

		PotSnapshot lastSnapshot = await unitOfWork.PotSnapshotRepository.GetLatestByPotIdAsync(pot.Id, cancellationToken);

		List<Gem> gems = await unitOfWork.GemRepository
			.FindAsync(new GemFilter
			{
				PotId = pot.Id,
				StartDate = lastSnapshot?.Date
			}, cancellationToken)
			.OrderBy(x => x.Date)
			.ToListAsync(cancellationToken);

		if (gems.Count == 0)
			return response;

		decimal startingValue = lastSnapshot?.Value ?? 0;

		DateOnly firstMonthStart = lastSnapshot != null
			? FirstDayOfMonth(lastSnapshot.Date).AddMonths(1)
			: FirstDayOfMonth(gems[0].Date.ToDateOnly());

		DateOnly lastMonthStart = FirstDayOfMonth(gems[^1].Date.ToDateOnly());

		if (firstMonthStart > lastMonthStart)
			return response;

		List<PotSnapshot> newSnapshots = BuildMonthlySnapshots(pot, startingValue, gems, firstMonthStart, lastMonthStart);

		try
		{
			unitOfWork.PotSnapshotRepository.AddRange(newSnapshots);
			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}

		response.Items.AddRange(newSnapshots
			.Select(x => new CreatedSnapshotItem
			{
				Date = x.Date,
				Value = new Amount
				{
					Currency = pot.Currency,
					Value = x.Value
				}
			}));

		return response;
	}

	private static List<PotSnapshot> BuildMonthlySnapshots(Pot pot, decimal startingValue, List<Gem> gems, DateOnly firstMonthStart, DateOnly lastMonthStart)
	{
		List<PotSnapshot> newSnapshots = [];

		decimal runningValue = startingValue;
		int gemIndex = 0;

		for (DateOnly monthStart = firstMonthStart; monthStart <= lastMonthStart; monthStart = monthStart.AddMonths(1))
		{
			DateOnly monthEnd = LastDayOfMonth(monthStart);

			while (gemIndex < gems.Count && gems[gemIndex].Date.ToDateOnly() <= monthEnd)
			{
				runningValue += CalculateGemAmount(gems[gemIndex]);
				gemIndex++;
			}

			newSnapshots.Add(new PotSnapshot
			{
				Date = monthEnd,
				Value = runningValue,
				IsAutomatic = true,
				Pot = pot
			});
		}

		return newSnapshots;
	}

	private static decimal CalculateGemAmount(Gem gem)
	{
		return gem.Category switch
		{
			GemCategory.Deposit or GemCategory.Gain or GemCategory.Bonus => gem.Amount,
			GemCategory.Withdrawal or GemCategory.Fee or GemCategory.Tax => -gem.Amount,
			_ => 0
		};
	}

	private static DateOnly FirstDayOfMonth(DateOnly date)
	{
		return new DateOnly(date.Year, date.Month, 1);
	}

	private static DateOnly LastDayOfMonth(DateOnly monthStart)
	{
		int daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);
		return new DateOnly(monthStart.Year, monthStart.Month, daysInMonth);
	}
}