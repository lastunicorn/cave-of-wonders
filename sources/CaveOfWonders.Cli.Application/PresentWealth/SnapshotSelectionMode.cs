namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal enum SnapshotSelectionMode
{
	Closest = 0,
	LastAvailable,
	LastAvailableAllowNext,
	NextAvailable,
	NextAvailableAllowLast
}