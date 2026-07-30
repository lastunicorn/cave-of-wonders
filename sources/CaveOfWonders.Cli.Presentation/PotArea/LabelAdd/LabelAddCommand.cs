using DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

[NamedCommand("label-add", Description = "Add a label to a pot. If the identifier matches multiple pots, the label is added to all of them.")]
internal class LabelAddCommand : IConsoleCommand<LabelAddViewModel>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(DisplayName = "Label", Order = 1, IsMandatory = true, Description = "The label to add to the pot.")]
	public string Label { get; set; }

	[NamedParameter("pot", ShortName = 'p', IsMandatory = true, Description = "Name or id of the pot. Partial id is accepted.")]
	public PotFlexId PotIdentifier { get; set; }

	public LabelAddCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<LabelAddViewModel> Execute()
	{
		AddPotLabelRequest request = new()
		{
			PotId = PotIdentifier,
			Label = Label
		};

		AddPotLabelResponse response = await requestBus.SendAsync<AddPotLabelRequest, AddPotLabelResponse>(request);

		return new LabelAddViewModel(response);
	}
}