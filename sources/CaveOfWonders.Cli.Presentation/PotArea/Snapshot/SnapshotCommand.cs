using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Snapshot;

[NamedCommand("snapshot", Description = "Display the list of snapshots for a specific pot.")]
internal class SnapshotCommand : IConsoleCommand<SnapshotViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = false, Description = "Name or id of the pot. Partial id is accepted.")]
	public string PotIdentifier { get; set; }

	[NamedParameter("pot", IsMandatory = false, Description = "Name or id of the pot for which to display the snapshots.")]
	public string PotId { get; set; }

	public SnapshotCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<SnapshotViewModel> Execute()
	{
		PresentPotSnapshotsRequest request = new()
		{
			PotFlexId = PotIdentifier ?? PotId
		};

		PresentPotSnapshotsResponse response = await mediator.Send(request);

		return new SnapshotViewModel
		{
			PotSummaries = response.PotSummaries?
				.Select(x => new PotSummaryViewModel(x))
				.ToList(),
			Pot = response.Pot is null
				? null
				: new PotSummaryViewModel(response.Pot),
			Snapshots = response.Snapshots?
				.Select(x => new PotSnapshotViewModel(x))
				.ToList()
		};
	}
}
