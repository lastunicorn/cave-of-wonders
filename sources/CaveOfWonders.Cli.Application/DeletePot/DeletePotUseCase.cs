using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.UserAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeletePot;

internal class DeletePotUseCase : IRequestHandler<DeletePotRequest, DeletePotResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IUserInterface userInterface;

	public DeletePotUseCase(IUnitOfWork unitOfWork, IUserInterface userInterface)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
	}

	public async Task<DeletePotResponse> Handle(DeletePotRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await RetrievePot(request.PotId, cancellationToken);

		if (pot == null)
		{
			return new DeletePotResponse
			{
				PotFound = false
			};
		}

		if (!request.Confirmed && !userInterface.ConfirmPotDelete(pot.Name))
		{
			return new DeletePotResponse
			{
				Cancelled = true
			};
		}

		await DeletePotWithRelatedData(pot, cancellationToken);

		return new DeletePotResponse
		{
			PotFound = true,
			PotName = pot.Name
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

		return matchedPot;
	}

	private async Task DeletePotWithRelatedData(Pot pot, CancellationToken cancellationToken)
	{
		try
		{
			List<Gem> gems = await unitOfWork.GemRepository.GetByPotIdAsync(pot.Id, cancellationToken)
				.ToListAsync(cancellationToken);

			foreach (Gem gem in gems)
				unitOfWork.GemRepository.Remove(gem);

			unitOfWork.PotRepository.Remove(pot);

			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new StorageInaccessibleException(ex);
		}
	}
}