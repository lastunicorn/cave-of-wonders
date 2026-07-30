using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

internal class PresentPotLabelsUseCase : IUseCase<PresentPotLabelsRequest, PresentPotLabelsResponse>
{
	private readonly ISystemClock systemClock;
	private readonly OperationManager operationManager;

	public PresentPotLabelsUseCase(ISystemClock systemClock, OperationManager operationManager)
	{
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<PresentPotLabelsResponse> Execute(PresentPotLabelsRequest request, CancellationToken cancellationToken)
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
			IAsyncEnumerable<Pot> pots = operationManager.ExecuteStream<GetPotsOperation, Pot>(
				op =>
				{
					op.PotId = request.PotFlexId;
					op.IncludeInactive = request.IncludeInactivePots;
					op.Today = today;
				},
				cancellationToken);

			return await pots.ToListAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}
}