using DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

[NamedCommand("label-add", Description = "Add a label to a pot. If the identifier matches multiple pots, the label is added to all of them.")]
internal class LabelAddCommand : IConsoleCommand<LabelAddViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(DisplayName = "Label", Order = 1, IsMandatory = true, Description = "The label to add to the pot.")]
	public string Label { get; set; }

	[NamedParameter("pot", ShortName = 'p', IsMandatory = true, Description = "Name or id of the pot. Partial id is accepted.")]
	public string PotIdentifier { get; set; }

	public LabelAddCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<LabelAddViewModel> Execute()
	{
		AddPotLabelRequest request = new()
		{
			PotId = PotIdentifier,
			Label = Label
		};

		AddPotLabelResponse response = await mediator.Send(request);

		return new LabelAddViewModel(response);
	}
}