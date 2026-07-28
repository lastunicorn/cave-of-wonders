namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

internal enum SnapshotSelectionMode
{
	LastAvailable,
	LastAvailableAllowNext,
	NextAvailable,
	NextAvailableAllowLast,
	Closest
}