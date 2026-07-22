using DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotCreate;

[NamedCommand("pot-create", Description = "Create a new pot.")]
internal class PotCreateCommand : IConsoleCommand<PotCreateViewModel>
{
	private readonly IMediator mediator;

	[NamedParameter("name", ShortName = 'n', Description = "The name of the new pot.")]
	public string Name { get; set; }

	[NamedParameter("description", ShortName = 'd', IsMandatory = false, Description = "Optional description for the pot.")]
	public string Description { get; set; }

	[NamedParameter("start-date", ShortName = 's', IsMandatory = false, Description = "Optional start date for the pot. Default value = today")]
	public DateOnly? StartDate { get; set; }

	[NamedParameter("currency", ShortName = 'c', Description = "The currency for this pot.")]
	public string Currency { get; set; }

	public PotCreateCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<PotCreateViewModel> Execute()
	{
		CreatePotRequest request = new()
		{
			Name = Name,
			Description = Description,
			StartDate = StartDate,
			Currency = Currency
		};

		CreatePotResponse response = await mediator.Send(request);

		return new PotCreateViewModel
		{
			PotName = response.Name,
			Description = response.Description,
			StartDate = response.StartDate,
			Currency = response.Currency,
			PotId = response.PotId
		};
	}
}