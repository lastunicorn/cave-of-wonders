using DustInTheWind.CaveOfWonders.Cli.Application.CreateSnapshots;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotCreate;

[NamedCommand("snapshot-create", Description = "Automatically create monthly snapshots for a pot, from the latest existing snapshot up to the month of the last gem.")]
internal class SnapshotCreateCommand : IConsoleCommand<SnapshotCreateViewModel>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = true, Description = "Name or id of the pot. Partial id is accepted.")]
	public PotFlexId PotIdentifier { get; set; }

	public SnapshotCreateCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<SnapshotCreateViewModel> Execute()
	{
		CreateSnapshotsRequest request = new()
		{
			PotId = PotIdentifier
		};

		CreateSnapshotsResponse response = await requestBus.SendAsync<CreateSnapshotsRequest, CreateSnapshotsResponse>(request);

		return new SnapshotCreateViewModel(response);
	}
}
