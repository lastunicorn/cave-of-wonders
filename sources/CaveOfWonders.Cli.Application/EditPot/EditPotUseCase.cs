using DustInTheWind.CaveOfWonders.Cli.Application.Operations;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.OperationEngine;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.EditPot;

internal class EditPotUseCase : IUseCase<EditPotRequest, EditPotResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly OperationManager operationManager;

	public EditPotUseCase(IUnitOfWork unitOfWork, OperationManager operationManager)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.operationManager = operationManager ?? throw new ArgumentNullException(nameof(operationManager));
	}

	public async Task<EditPotResponse> Execute(EditPotRequest request, CancellationToken cancellationToken)
	{
		Pot pot = await operationManager.ExecuteAsync<GetOnePotOperation, Pot>(
			op =>
			{
				op.PotId = request.PotId;
			},
			cancellationToken);

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

		if (request.StartDate.HasValue)
		{
			response.StartDateUpdated = true;
			response.OldStartDate = pot.StartDate;
			pot.StartDate = request.StartDate.Value;
			response.NewStartDate = pot.StartDate;
		}

		if (request.EndDate.HasValue)
		{
			response.EndDateUpdated = true;
			response.OldEndDate = pot.EndDate;
			pot.EndDate = request.EndDate.Value;
			response.NewEndDate = pot.EndDate;
		}

		if (response.NameUpdated || response.DescriptionUpdated || response.CurrencyUpdated || response.StartDateUpdated || response.EndDateUpdated)
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
}
