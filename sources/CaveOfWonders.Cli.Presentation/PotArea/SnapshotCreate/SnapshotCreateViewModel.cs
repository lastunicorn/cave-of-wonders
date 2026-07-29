using DustInTheWind.CaveOfWonders.Cli.Application.CreateSnapshots;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.SnapshotCreate;

internal class SnapshotCreateViewModel
{
	public string PotName { get; }

	public List<CreatedSnapshotItemViewModel> Items { get; }

	internal SnapshotCreateViewModel(CreateSnapshotsResponse response)
	{
		PotName = response.PotName;
		Items = response.Items
			.Select(x => new CreatedSnapshotItemViewModel(x))
			.ToList();
	}
}
