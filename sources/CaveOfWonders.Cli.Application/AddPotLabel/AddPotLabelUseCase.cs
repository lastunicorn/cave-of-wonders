using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

internal class AddPotLabelUseCase : IRequestHandler<AddPotLabelRequest, AddPotLabelResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public AddPotLabelUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<AddPotLabelResponse> Handle(AddPotLabelRequest request, CancellationToken cancellationToken)
	{
		string label = request.Label.Trim().ToLowerInvariant();

		List<Pot> pots = await RetrievePots(request.PotId, cancellationToken);

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
					PotName = x.Name
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
