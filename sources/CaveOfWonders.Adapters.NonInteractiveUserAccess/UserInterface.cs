using DustInTheWind.CaveOfWonders.Ports.UserAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.NonInteractiveUserAccess;

public class UserInterface : IUserInterface
{
	public bool ConfirmPotDelete(string potName)
	{
		return true;
	}

	public bool ConfirmSnapshotsDelete(string potName, int snapshotCount, DateOnly? startDate = null, DateOnly? endDate = null)
	{
		return true;
	}
}