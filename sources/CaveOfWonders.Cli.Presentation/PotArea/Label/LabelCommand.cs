using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

[NamedCommand("label", Description = "Display the labels of a specific pot, or of all pots if none is specified.")]
internal class LabelCommand : IConsoleCommand<LabelViewModel>
{
	private readonly RequestBus requestBus;

	[NamedParameter("pot", ShortName = 'p', IsMandatory = false, Description = "Name or id of the pot. Partial id is accepted.")]
	public string PotIdentifier { get; set; }

	[NamedParameter("all", ShortName = 'a', IsMandatory = false, Description = "Display all pots, including the inactive ones. Default = false.")]
	public bool IncludeInactivePots { get; set; }

	public LabelCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<LabelViewModel> Execute()
	{
		PresentPotLabelsRequest request = new()
		{
			PotFlexId = PotIdentifier,
			IncludeInactivePots = IncludeInactivePots
		};

		PresentPotLabelsResponse response = await requestBus.SendAsync<PresentPotLabelsRequest, PresentPotLabelsResponse>(request);

		return new LabelViewModel(response);
	}
}