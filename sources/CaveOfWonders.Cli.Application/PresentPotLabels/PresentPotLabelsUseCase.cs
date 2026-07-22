using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

internal class PresentPotLabelsUseCase : IRequestHandler<PresentPotLabelsRequest, PresentPotLabelsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public PresentPotLabelsUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<PresentPotLabelsResponse> Handle(PresentPotLabelsRequest request, CancellationToken cancellationToken)
	{
		DateOnly today = systemClock.Today;

		List<Pot> pots = await RetrievePots(request, today, cancellationToken);

		return new PresentPotLabelsResponse
		{
			Items = pots
				.Select(x => new PotLabelsItem
				{
					PotId = x.Id,
					PotName = x.Name,
					Labels = x.Labels
						.Select(l => l.Label)
						.ToList(),
					IsActive = x.IsActive(today)
				})
				.ToList()
		};
	}

	private async Task<List<Pot>> RetrievePots(PresentPotLabelsRequest request, DateOnly today, CancellationToken cancellationToken)
	{
		try
		{
			bool isIdentifierSpecified = request.PotFlexId?.HasValue == true;

			IAsyncEnumerable<Pot> pots = isIdentifierSpecified
				? unitOfWork.PotRepository.GetAsync(request.PotFlexId, cancellationToken)
				: unitOfWork.PotRepository.GetAllAsync(cancellationToken);

			if (!request.IncludeInactivePots)
				pots = pots.Where(x => x.IsActive(today));

			return await pots
				.OrderBy(x => x.DisplayOrder)
				.ToListAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}
}
