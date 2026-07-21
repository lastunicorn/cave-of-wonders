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
		IAsyncEnumerable<Gem> gems = RetrieveGems(pot.Id, request, cancellationToken);

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

	private IAsyncEnumerable<Gem> RetrieveGems(Guid potId, PresentGemsRequest request, CancellationToken cancellationToken)
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

		MonthAndYear month = DecideMonth(request);

		if (month.HasValue)
			filter.Month = month;

		if (!request.StartDate.HasValue && !request.EndDate.HasValue && !request.Date.HasValue && !month.HasValue)
			filter.Month = new MonthAndYear(systemClock.Today);

		if (request.ExcludeInternal)
			filter.ExcludeCategories = [GemCategory.Internal];

		return unitOfWork.GemRepository.FindAsync(filter, cancellationToken);
	}

	private MonthAndYear DecideMonth(PresentGemsRequest request)
	{
		if (request.CurrentMonth)
			return new MonthAndYear(systemClock.Today);

		if (request.LastMonth)
			return new MonthAndYear(systemClock.Today.AddMonths(-1));

		return request.Month;
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