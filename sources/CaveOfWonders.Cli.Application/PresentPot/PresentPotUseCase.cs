using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

internal class PresentPotUseCase : IUseCase<PresentPotRequest, PresentPotResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly OperationManager operationManager;

	public PresentPotUseCase(IUnitOfWork unitOfWork, OperationManager operationManager)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<PresentPotResponse> Execute(PresentPotRequest request, CancellationToken cancellationToken)
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
			IAsyncEnumerable<Pot> pots = operationManager.ExecuteStream<GetPotsOperation, Pot>(
				op =>
				{
					op.PotId = request.PotFlexId;
					op.IncludeInactive = request.IncludeInactivePots;
				},
				cancellationToken);

			return await pots.ToListAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}

	private async Task<List<PotDetails>> BuildPotDetails(IEnumerable<Pot> pots, CancellationToken cancellationToken)
	{
		List<PotDetails> potDetailsList = [];

		foreach (Pot pot in pots)
		{
			int gemCount = await unitOfWork.GemRepository.GetCountAsync(pot.Id, cancellationToken);

			Gem latestGem = await unitOfWork.GemRepository.GetLatestAsync(pot.Id, cancellationToken);
			DateOnly? latestGemDate = latestGem != null
				? DateOnly.FromDateTime(latestGem.Date)
				: null;

			int snapshotCount = await unitOfWork.PotSnapshotRepository.GetCountAsync(pot.Id, cancellationToken: cancellationToken);
			PotSnapshot latestSnapshot = await unitOfWork.PotSnapshotRepository.GetLatestByPotIdAsync(pot.Id, cancellationToken);

			potDetailsList.Add(new PotDetails(pot, gemCount, latestGemDate, snapshotCount, latestSnapshot));
		}

		return potDetailsList;
	}
}