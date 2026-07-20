using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

internal class PresentGemsUseCase : IRequestHandler<PresentGemsRequest, PresentGemsResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public PresentGemsUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

		if (request.Date.HasValue)
			filter.Date = request.Date.Value;

		if (request.Month.HasValue)
			filter.Month = request.Month;

		if (request.ExcludeInternal)
			filter.ExcludeCategories = [GemCategory.Internal];

		return unitOfWork.GemRepository.FindAsync(filter, cancellationToken);
	}

	private static decimal CalculateAmount(Gem gem)
	{
		return gem.Category switch
		{
			GemCategory.Unknown => gem.Amount,
			GemCategory.Deposit => gem.Amount,
			GemCategory.Withdrawal => -gem.Amount,
			GemCategory.Internal => gem.Amount,
			GemCategory.Gain => gem.Amount,
			GemCategory.Fee => -gem.Amount,
			GemCategory.Tax => -gem.Amount,
			GemCategory.Bonus => gem.Amount,
			_ => 0
		};
	}
}