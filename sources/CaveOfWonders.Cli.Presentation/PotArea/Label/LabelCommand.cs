using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

[NamedCommand("label", Description = "Display the labels of a specific pot, or of all pots if none is specified.")]
internal class LabelCommand : IConsoleCommand<LabelViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = false, Description = "Name or id of the pot. Partial id is accepted.")]
	public string PotIdentifier { get; set; }

	public LabelCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<LabelViewModel> Execute()
	{
		PresentPotLabelsRequest request = new()
		{
			PotFlexId = PotIdentifier
		};

		PresentPotLabelsResponse response = await mediator.Send(request);

		return new LabelViewModel(response);
	}
}
