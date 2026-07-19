using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;

public class CreatePotUseCase : IRequestHandler<CreatePotRequest, CreatePotResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public CreatePotUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<CreatePotResponse> Handle(CreatePotRequest request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Name))
			throw new PotNameNotSpecifiedException();

		if (string.IsNullOrWhiteSpace(request.Currency))
			throw new PotCurrencyNotSpecifiedException();

		DateOnly startDate = request.StartDate ?? systemClock.Today;

		Pot pot = new()
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Description = request.Description,
			StartDate = startDate,
			Currency = request.Currency
		};

		try
		{
			unitOfWork.PotRepository.Add(pot);
			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new StorageInaccessibleException(ex);
		}

		return new CreatePotResponse
		{
			PotId = pot.Id,
			Name = pot.Name,
			Description = pot.Description,
			StartDate = pot.StartDate,
			Currency = pot.Currency
		};
	}
}