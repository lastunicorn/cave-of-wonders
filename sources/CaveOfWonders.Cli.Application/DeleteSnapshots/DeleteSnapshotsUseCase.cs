using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.UserAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.DeleteSnapshots;

internal class DeleteSnapshotsUseCase : IUseCase<DeleteSnapshotsRequest, DeleteSnapshotsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IUserInterface userInterface;

	public DeleteSnapshotsUseCase(IUnitOfWork unitOfWork, IUserInterface userInterface)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
	}

	public async Task<DeleteSnapshotsResponse> Execute(DeleteSnapshotsRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await RetrievePot(request.PotId, cancellationToken);

		int snapshotCount = await unitOfWork.PotSnapshotRepository.GetCountAsync(pot.Id, cancellationToken);

		if (snapshotCount == 0)
		{
			return new DeleteSnapshotsResponse
			{
				PotName = pot.Name,
				DeletedCount = 0
			};
		}

		if (!request.Confirmed && !userInterface.ConfirmSnapshotsDelete(pot.Name, snapshotCount))
		{
			return new DeleteSnapshotsResponse
			{
				PotName = pot.Name,
				Cancelled = true
			};
		}

		await DeleteSnapshots(pot, cancellationToken);

		return new DeleteSnapshotsResponse
		{
			PotName = pot.Name,
			DeletedCount = snapshotCount
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

		if (matchedPot == null)
			throw new PotNotFoundException(potId);

		return matchedPot;
	}

	private async Task DeleteSnapshots(Pot pot, CancellationToken cancellationToken)
	{
		try
		{
			unitOfWork.PotSnapshotRepository.RemoveByPotId(pot.Id);

			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}
}
