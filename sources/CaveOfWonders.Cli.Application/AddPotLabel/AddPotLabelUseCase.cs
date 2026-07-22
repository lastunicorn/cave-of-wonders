using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

internal class AddPotLabelUseCase : IRequestHandler<AddPotLabelRequest, AddPotLabelResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public AddPotLabelUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<AddPotLabelResponse> Handle(AddPotLabelRequest request, CancellationToken cancellationToken)
	{
		DateOnly today = systemClock.Today;

		string label = request.Label.Trim().ToLowerInvariant();

		List<Pot> pots = await RetrievePots(request.PotId, cancellationToken);

		Dictionary<Guid, bool> wasAddedByPotId = new();

		foreach (Pot pot in pots)
		{
			bool alreadyHasLabel = pot.Labels.Any(x => x.Label == label);

			if (!alreadyHasLabel)
			{
				pot.Labels.Add(new PotLabel
				{
					Label = label
				});
			}

			wasAddedByPotId[pot.Id] = !alreadyHasLabel;
		}

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
			Items = pots
				.Select(x => new LabelAddResult
				{
					PotId = x.Id,
					PotName = x.Name,
					WasAdded = wasAddedByPotId[x.Id],
					IsActive = x.IsActive(today)
				})
				.ToList()
		};
	}

	private async Task<List<Pot>> RetrievePots(PotFlexId potId, CancellationToken cancellationToken)
	{
		List<Pot> pots;

		try
		{
			pots = await unitOfWork.PotRepository.GetAsync(potId, cancellationToken).ToListAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}

		if (pots.Count == 0)
			throw new PotNotFoundException(potId);

		return pots;
	}
}
