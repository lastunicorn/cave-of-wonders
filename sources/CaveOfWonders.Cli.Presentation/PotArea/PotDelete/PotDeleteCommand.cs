using DustInTheWind.CaveOfWonders.Cli.Application.DeletePot;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.InputControls;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotDelete;

[NamedCommand("pot-delete", Description = "Delete a pot together with its gems and snapshots.")]
internal class PotDeleteCommand : IConsoleCommand<PotDeleteViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = true, Description = "Name or id of the pot to delete.")]
	public string PotIdentifier { get; set; }

	[NamedParameter("yes", ShortName = 'y', IsMandatory = false, Description = "Delete the pot without asking for confirmation.")]
	public bool Confirmed { get; set; }

	public PotDeleteCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<PotDeleteViewModel> Execute()
	{
		DeletePotRequest request = new()
		{
			PotId = PotIdentifier,
			Confirmed = Confirmed
		};

		DeletePotResponse response = await mediator.Send(request);

		return new PotDeleteViewModel
		{
			PotFound = response.PotFound,
			PotName = response.PotName,
			Cancelled = response.Cancelled
		};
	}
}