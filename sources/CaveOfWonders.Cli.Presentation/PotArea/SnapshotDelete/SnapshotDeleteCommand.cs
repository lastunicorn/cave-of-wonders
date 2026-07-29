using DustInTheWind.CaveOfWonders.Cli.Application.DeleteSnapshots;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotDelete;

[NamedCommand("snapshot-delete", Description = "Delete the snapshots of a pot.")]
internal class SnapshotDeleteCommand : IConsoleCommand<SnapshotDeleteViewModel>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = true, Description = "Name or id of the pot whose snapshots to delete. Partial id is accepted.")]
	public string PotIdentifier { get; set; }

	[NamedParameter("start-date", IsMandatory = false, Description = "Delete only the snapshots starting with this date. If not specified, there is no lower limit.")]
	public DateOnly? StartDate { get; set; }

	[NamedParameter("end-date", IsMandatory = false, Description = "Delete only the snapshots up to this date. If not specified, there is no upper limit.")]
	public DateOnly? EndDate { get; set; }

	[NamedParameter("yes", ShortName = 'y', IsMandatory = false, Description = "Delete the snapshots without asking for confirmation.")]
	public bool Confirmed { get; set; }

	public SnapshotDeleteCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<SnapshotDeleteViewModel> Execute()
	{
		DeleteSnapshotsRequest request = new()
		{
			PotId = PotIdentifier,
			StartDate = StartDate,
			EndDate = EndDate,
			Confirmed = Confirmed
		};

		DeleteSnapshotsResponse response = await requestBus.SendAsync<DeleteSnapshotsRequest, DeleteSnapshotsResponse>(request);

		return new SnapshotDeleteViewModel
		{
			PotName = response.PotName,
			StartDate = response.StartDate,
			EndDate = response.EndDate,
			DeletedCount = response.DeletedCount,
			Cancelled = response.Cancelled
		};
	}
}
