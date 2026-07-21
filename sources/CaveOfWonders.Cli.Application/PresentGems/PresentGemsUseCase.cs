using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

internal class PresentGemsUseCase : IRequestHandler<PresentGemsRequest, PresentGemsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public PresentGemsUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<PresentGemsResponse> Handle(PresentGemsRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await RetrievePot(request.PotId, cancellationToken);
		IAsyncEnumerable<Gem> gems = await RetrieveGems(pot.Id, request, cancellationToken);

		SortedList<DateTime, GemDto> gemsByDate = new(new DuplicateKeyComparer<DateTime>());
		decimal totalAmount = 0;

		await foreach (Gem gem in gems)
		{
			GemDto gemDto = ConvertToDto(gem);

			gemsByDate.Add(gem.Date, gemDto);

			if (gem.Category is not (GemCategory.Unknown or GemCategory.Internal))
				totalAmount += gemDto.Amount.Value;
		}

		return new PresentGemsResponse
		{
			Gems = gemsByDate.Values.AsReadOnly(),
			TotalAmount = new Amount
			{
				Currency = pot.Currency,
				Value = totalAmount
			}
		};
	}

	private static GemDto ConvertToDto(Gem gem)
	{
		return new GemDto
		{
			Date = gem.Date,
			Category = gem.Category,
			Amount = new Amount
			{
				Currency = gem.Pot?.Currency,
				Value = CalculateAmount(gem)
			}
		};
	}

	private async Task<Pot> RetrievePot(PotFlexId potId, CancellationToken cancellationToken)
	{
		IAsyncEnumerable<Pot> pots = unitOfWork.PotRepository.GetAsync(potId, cancellationToken);

		Pot matchedPot = null;

		await foreach (Pot pot in pots)
		{
			if (matchedPot != null)
				throw new MultiplePotsException(potId);

			if (pot != null)
				matchedPot = pot;
		}

		if (matchedPot == null)
			throw new PotNotFoundException(potId);

		return matchedPot;
	}

	private async Task<IAsyncEnumerable<Gem>> RetrieveGems(Guid potId, PresentGemsRequest request, CancellationToken cancellationToken)
	{
		GemFilter filter = new()
		{
			PotId = potId,
		};

		if (request.StartDate.HasValue)
			filter.StartDate = request.StartDate.Value;

		if (request.EndDate.HasValue)
			filter.EndDate = request.EndDate.Value;

		if (request.Date.HasValue)
			filter.Date = request.Date.Value;

		MonthAndYear month = await DecideMonth(potId, request, cancellationToken);

		if (month.HasValue)
			filter.Month = month;

		int? year = await DecideYear(potId, request, cancellationToken);

		if (year.HasValue && !request.StartDate.HasValue && !request.EndDate.HasValue)
		{
			filter.StartDate = new DateOnly(year.Value, 1, 1);
			filter.EndDate = new DateOnly(year.Value, 12, 31);
		}

		if (!request.StartDate.HasValue && !request.EndDate.HasValue && !request.Date.HasValue && !month.HasValue && !year.HasValue)
			filter.Month = new MonthAndYear(systemClock.Today);

		if (request.ExcludeInternal)
			filter.ExcludeCategories = [GemCategory.Internal];

		return unitOfWork.GemRepository.FindAsync(filter, cancellationToken);
	}

	private async Task<MonthAndYear> DecideMonth(Guid potId, PresentGemsRequest request, CancellationToken cancellationToken)
	{
		if (request.CurrentMonth)
			return new MonthAndYear(systemClock.Today);

		if (request.LastMonth)
			return new MonthAndYear(systemClock.Today.AddMonths(-1));

		if (request.LatestMonth)
			return await RetrieveLatestMonthWithGems(potId, cancellationToken);

		return request.Month;
	}

	private async Task<MonthAndYear> RetrieveLatestMonthWithGems(Guid potId, CancellationToken cancellationToken)
	{
		Gem latestGem = await unitOfWork.GemRepository.GetLatestAsync(potId, cancellationToken);

		return latestGem == null
			? default
			: new MonthAndYear(DateOnly.FromDateTime(latestGem.Date));
	}

	private async Task<int?> DecideYear(Guid potId, PresentGemsRequest request, CancellationToken cancellationToken)
	{
		if (request.CurrentYear)
			return systemClock.Today.Year;

		if (request.LastYear)
			return systemClock.Today.Year - 1;

		if (request.LatestYear)
			return await RetrieveLatestYearWithGems(potId, cancellationToken);

		return null;
	}

	private async Task<int?> RetrieveLatestYearWithGems(Guid potId, CancellationToken cancellationToken)
	{
		Gem latestGem = await unitOfWork.GemRepository.GetLatestAsync(potId, cancellationToken);

		return latestGem?.Date.Year;
	}

	private static decimal CalculateAmount(Gem gem)
	{
		return gem.Category switch
		{
			GemCategory.Withdrawal => -gem.Amount,
			GemCategory.Fee => -gem.Amount,
			GemCategory.Tax => -gem.Amount,
			_ => gem.Amount
		};
	}
}