using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.UserAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeletePot;

internal class DeletePotUseCase : IUseCase<DeletePotRequest, DeletePotResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IUserInterface userInterface;
	private readonly OperationManager operationManager;

	public DeletePotUseCase(IUnitOfWork unitOfWork, IUserInterface userInterface, OperationManager operationManager)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<DeletePotResponse> Execute(DeletePotRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await operationManager.CreateAndExecuteAsync<GetOnePotOperation, Pot>(
			op =>
			{
				op.PotId = request.PotId;
				op.ThrowIfNotFound = false;
			},
			cancellationToken);

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

	private async Task DeletePotWithRelatedData(Pot pot, CancellationToken cancellationToken)
	{
		try
		{
			List<Gem> gems = await unitOfWork.GemRepository.GetByPotIdAsync(pot.Id, cancellationToken)
				.ToListAsync(cancellationToken);

			foreach (Gem gem in gems)
				unitOfWork.GemRepository.Remove(gem);

			unitOfWork.PotSnapshotRepository.RemoveByPotId(pot.Id);

			unitOfWork.PotRepository.Remove(pot);

			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}
}