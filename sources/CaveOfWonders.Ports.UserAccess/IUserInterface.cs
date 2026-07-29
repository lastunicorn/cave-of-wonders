namespace DustInTheWind.CaveOfWonders.Ports.UserAccess;

public interface IUserInterface
{
	bool ConfirmPotDelete(string potName);

	bool ConfirmSnapshotsDelete(string potName, int snapshotCount);
}