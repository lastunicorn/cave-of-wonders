using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.UserAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeleteSnapshots;

internal class DeleteSnapshotsUseCase : IUseCase<DeleteSnapshotsRequest, DeleteSnapshotsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IUserInterface userInterface;
	private readonly OperationManager operationManager;

	public DeleteSnapshotsUseCase(IUnitOfWork unitOfWork, IUserInterface userInterface, OperationManager operationManager)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<DeleteSnapshotsResponse> Execute(DeleteSnapshotsRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await operationManager.ExecuteAsync<GetOnePotOperation, Pot>(
			op =>
			{
				op.PotId = request.PotId;
			},
			cancellationToken);

		int snapshotCount = await unitOfWork.PotSnapshotRepository.GetCountAsync(pot.Id, request.StartDate, request.EndDate, cancellationToken);

		if (snapshotCount == 0)
		{
			return new DeleteSnapshotsResponse
			{
				PotName = pot.Name,
				StartDate = request.StartDate,
				EndDate = request.EndDate,
				DeletedCount = 0
			};
		}

		if (!request.Confirmed && !userInterface.ConfirmSnapshotsDelete(pot.Name, snapshotCount, request.StartDate, request.EndDate))
		{
			return new DeleteSnapshotsResponse
			{
				PotName = pot.Name,
				StartDate = request.StartDate,
				EndDate = request.EndDate,
				Cancelled = true
			};
		}

		await DeleteSnapshots(pot, request.StartDate, request.EndDate, cancellationToken);

		return new DeleteSnapshotsResponse
		{
			PotName = pot.Name,
			StartDate = request.StartDate,
			EndDate = request.EndDate,
			DeletedCount = snapshotCount
		};
	}

	private async Task DeleteSnapshots(Pot pot, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
	{
		try
		{
			unitOfWork.PotSnapshotRepository.RemoveByPotId(pot.Id, startDate, endDate);

			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}
}
