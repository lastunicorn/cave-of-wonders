using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.EditPot;

internal class EditPotUseCase : IRequestHandler<EditPotRequest, EditPotResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public EditPotUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<EditPotResponse> Handle(EditPotRequest request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Name))
			throw new PotNameNotSpecifiedException();

		Pot pot = await RetrievePot(request.PotId, cancellationToken);

		string oldName = pot.Name;
		pot.Name = request.Name;

		try
		{
			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}

		return new EditPotResponse
		{
			PotId = pot.Id,
			OldName = oldName,
			NewName = pot.Name
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
}