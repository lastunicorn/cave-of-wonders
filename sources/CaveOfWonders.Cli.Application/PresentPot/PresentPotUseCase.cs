using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

internal class PresentPotUseCase : IRequestHandler<PresentPotRequest, PresentPotResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public PresentPotUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<PresentPotResponse> Handle(PresentPotRequest request, CancellationToken cancellationToken)
	{
		IEnumerable<Pot> pots = await RetrievePots(request, cancellationToken);

		PresentPotResponse response = new();

		bool showDetails = request.ShowDetails is true || (!request.ShowDetails.HasValue && request.PotFlexId?.HasValue == true);
		if (showDetails)
		{
			response.PotDetails = await BuildPotDetails(pots, cancellationToken);
		}
		else
		{
			response.PotSummaries = pots
				.Select(x => new PotSummary(x))
				.ToList();
		}

		return response;
	}

	private async Task<IEnumerable<Pot>> RetrievePots(PresentPotRequest request, CancellationToken cancellationToken)
	{
		try
		{
			IAsyncEnumerable<Pot> pots = RetrievePots(request.PotFlexId, cancellationToken);

			if (!request.IncludeInactivePots)
			{
				DateOnly today = systemClock.Today;
				pots = pots.Where(x => x.IsActive(today));
			}

			pots = pots.OrderBy(x => x.DisplayOrder);

			return await pots.ToListAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new StorageInaccessibleException(ex);
		}
	}

	private IAsyncEnumerable<Pot> RetrievePots(PotFlexId potFlexId, CancellationToken cancellationToken)
	{
		bool isIdentifierSpecified = potFlexId?.HasValue == true;

		return isIdentifierSpecified
			? unitOfWork.PotRepository.GetAsync(potFlexId, cancellationToken)
			: unitOfWork.PotRepository.GetAllAsync(cancellationToken);
	}

	private async Task<List<PotDetails>> BuildPotDetails(IEnumerable<Pot> pots, CancellationToken cancellationToken)
	{
		List<PotDetails> potDetailsList = [];

		foreach (Pot pot in pots)
		{
			int gemCount = await unitOfWork.GemRepository.GetByPotIdAsync(pot.Id, cancellationToken)
				.CountAsync(cancellationToken);

			Gem lastGem = await unitOfWork.GemRepository.GetLatestAsync(pot.Id, cancellationToken);
			DateOnly? lastGemDate = lastGem != null
				? DateOnly.FromDateTime(lastGem.Date)
				: null;

			potDetailsList.Add(new PotDetails(pot, gemCount, lastGemDate));
		}

		return potDetailsList;
	}
}