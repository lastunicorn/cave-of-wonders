using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

internal class PresentPotSnapshotsUseCase : IRequestHandler<PresentPotSnapshotsRequest, PresentPotSnapshotsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public PresentPotSnapshotsUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<PresentPotSnapshotsResponse> Handle(PresentPotSnapshotsRequest request, CancellationToken cancellationToken)
	{
		if (request.PotFlexId?.HasValue != true)
			throw new PotFlexIdNotSpecifiedException();

		List<Pot> pots = await unitOfWork.PotRepository.GetAsync(request.PotFlexId, cancellationToken)
			.ToListAsync(cancellationToken);

		DateOnly today = systemClock.Today;

		if (pots.Count > 1)
		{
			return new PresentPotSnapshotsResponse
			{
				PotSummaries = pots
					.Select(x => new PotSummary(x, x.IsActive(today)))
					.ToList()
			};
		}

		if (pots.Count == 1)
		{
			Pot pot = pots[0];

			List<PotSnapshot> snapshots = await unitOfWork.PotSnapshotRepository
				.GetByPotIdAsync(pot.Id, request.StartDate, request.EndDate, cancellationToken)
				.ToListAsync(cancellationToken);

			return new PresentPotSnapshotsResponse
			{
				Pot = new PotSummary(pot, pot.IsActive(today)),
				Snapshots = snapshots
					.Select(x => new PotSnapshotItem
					{
						Date = x.Date,
						Value = new Amount
						{
							Currency = pot.Currency,
							Value = x.Value
						}
					})
					.ToList()
			};
		}

		return new PresentPotSnapshotsResponse();
	}
}