using DustInTheWind.CaveOfWonders.Cli.Application.EditPot;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotEdit;

[NamedCommand("pot-edit", Description = "Update the name of a pot.")]
internal class PotEditCommand : IConsoleCommand<PotEditViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = true, Description = "Name or id of the pot to edit.")]
	public string PotIdentifier { get; set; }

	[NamedParameter("name", ShortName = 'n', IsMandatory = true, Description = "The new name for the pot.")]
	public string Name { get; set; }

	public PotEditCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<PotEditViewModel> Execute()
	{
		EditPotRequest request = new()
		{
			PotId = PotIdentifier,
			Name = Name
		};

		EditPotResponse response = await mediator.Send(request);

		return new PotEditViewModel
		{
			PotId = response.PotId,
			OldName = response.OldName,
			NewName = response.NewName
		};
	}
}
