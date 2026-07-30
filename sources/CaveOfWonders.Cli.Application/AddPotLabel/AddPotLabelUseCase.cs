using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

internal class AddPotLabelUseCase : IUseCase<AddPotLabelRequest, AddPotLabelResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;
	private readonly OperationManager operationManager;

	public AddPotLabelUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock, OperationManager operationManager)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<AddPotLabelResponse> Execute(AddPotLabelRequest request, CancellationToken cancellationToken)
	{
		string label = request.Label.Trim().ToLowerInvariant();

		IAsyncEnumerable<Pot> pots = RetrievePots(request.PotId, cancellationToken);

		List<LabelAddResult> labelAddResults = await AddLabelToPots(pots, label);

		try
		{
			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}

		return new AddPotLabelResponse
		{
			Label = label,
			Items = labelAddResults
		};
	}

	private async Task<List<LabelAddResult>> AddLabelToPots(IAsyncEnumerable<Pot> pots, string label)
	{
		DateOnly today = systemClock.Today;

		List<LabelAddResult> labelAddResults = [];

		await foreach (Pot pot in pots)
		{
			bool alreadyHasLabel = pot.Labels.Any(x => x.Label == label);

			if (!alreadyHasLabel)
			{
				pot.Labels.Add(new PotLabel
				{
					Label = label
				});
			}

			labelAddResults.Add(new LabelAddResult
			{
				PotId = pot.Id,
				PotName = pot.Name,
				WasAdded = !alreadyHasLabel,
				IsActive = pot.IsActive(today)
			});
		}

		return labelAddResults;
	}

	private IAsyncEnumerable<Pot> RetrievePots(PotFlexId potId, CancellationToken cancellationToken)
	{
		return operationManager.ExecuteStream<GetPotsOperation, Pot>(
			op =>
			{
				op.PotId = potId;
				op.IncludeInactive = true;
			},
			cancellationToken);
	}
}