using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;

internal class SnapshotAddReport
{
	public SnapshotAddStatus AddStatus { get; set; }

	public Guid Key { get; set; }

	public Pot Pot { get; set; }

	public PotSnapshot PotSnapshot { get; set; }
}