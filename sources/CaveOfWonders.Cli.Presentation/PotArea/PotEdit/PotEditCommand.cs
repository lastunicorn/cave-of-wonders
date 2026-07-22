using DustInTheWind.CaveOfWonders.Cli.Application.EditPot;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotEdit;

[NamedCommand("pot-edit", Description = "Update the name, description and/or currency of a pot.")]
internal class PotEditCommand : IConsoleCommand<PotEditViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = true, Description = "Name or id of the pot to edit.")]
	public string PotIdentifier { get; set; }

	[NamedParameter("name", ShortName = 'n', IsMandatory = false, Description = "The new name for the pot.")]
	public string Name { get; set; }

	[NamedParameter("description", ShortName = 'd', IsMandatory = false, Description = "The new description for the pot.")]
	public string Description { get; set; }

	[NamedParameter("currency", ShortName = 'c', IsMandatory = false, Description = "The new currency for the pot.")]
	public string Currency { get; set; }

	public PotEditCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<PotEditViewModel> Execute()
	{
		EditPotRequest request = new()
		{
			PotId = PotIdentifier,
			Name = Name,
			Description = Description,
			Currency = Currency
		};

		EditPotResponse response = await mediator.Send(request);

		return new PotEditViewModel
		{
			PotId = response.PotId,
			PotName = response.PotName,
			NameUpdated = response.NameUpdated,
			OldName = response.OldName,
			NewName = response.NewName,
			DescriptionUpdated = response.DescriptionUpdated,
			OldDescription = response.OldDescription,
			NewDescription = response.NewDescription,
			CurrencyUpdated = response.CurrencyUpdated,
			OldCurrency = response.OldCurrency,
			NewCurrency = response.NewCurrency
		};
	}
}
