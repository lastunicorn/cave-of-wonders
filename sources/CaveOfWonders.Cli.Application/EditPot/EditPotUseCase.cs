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
		Pot pot = await RetrievePot(request.PotId, cancellationToken);

		EditPotResponse response = new();

		if (!string.IsNullOrWhiteSpace(request.Name))
		{
			response.NameUpdated = true;
			response.OldName = pot.Name;
			pot.Name = request.Name;
			response.NewName = pot.Name;
		}

		if (!string.IsNullOrWhiteSpace(request.Description))
		{
			response.DescriptionUpdated = true;
			response.OldDescription = pot.Description;
			pot.Description = request.Description;
			response.NewDescription = pot.Description;
		}

		if (!string.IsNullOrWhiteSpace(request.Currency))
		{
			response.CurrencyUpdated = true;
			response.OldCurrency = pot.Currency;
			pot.Currency = request.Currency;
			response.NewCurrency = pot.Currency;
		}

		if (response.NameUpdated || response.DescriptionUpdated || response.CurrencyUpdated)
		{
			try
			{
				await unitOfWork.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new DataStorageException(ex);
			}
		}

		response.PotId = pot.Id;
		response.PotName = pot.Name;

		return response;
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
