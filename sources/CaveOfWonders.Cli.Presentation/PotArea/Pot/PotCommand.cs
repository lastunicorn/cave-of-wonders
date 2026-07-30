using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pot;

[NamedCommand("pot", Description = "Display details about a specific pot.")]
internal class PotCommand : IConsoleCommand<PotCommandViewModel>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = false, Description = "Name or id of the pot. Partial id is accepted.")]
	public PotFlexId PotIdentifier { get; set; }

	[NamedParameter("all", ShortName = 'a', IsMandatory = false, Description = "Display all pots, including the inactive ones. Default = false.")]
	public bool IncludeInactivePots { get; set; }

	[NamedParameter("details", ShortName = 'd', IsMandatory = false, Description = "Display details about the pot. Default = false.")]
	public bool? ShowDetails { get; set; }

	public PotCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<PotCommandViewModel> Execute()
	{
		PresentPotRequest request = new()
		{
			PotFlexId = PotIdentifier,
			IncludeInactivePots = IncludeInactivePots,
			ShowDetails = ShowDetails
		};

		PresentPotResponse response = await requestBus.SendAsync<PresentPotRequest, PresentPotResponse>(request);

		return new PotCommandViewModel
		{
			PotDetails = response.PotDetails?
				.Select(x => new PotDetailsViewModel(x))
				.ToList(),
			PotSummaries = response.PotSummaries?
				.Select(x => new PotSummaryViewModel(x))
				.ToList()
		};
	}
}