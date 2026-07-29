using DustInTheWind.CaveOfWonders.Ports.UserAccess;
using DustInTheWind.ConsoleTools.Controls.InputControls;

namespace DustInTheWind.CaveOfWonders.Adapters.UserAccess;

public class UserInterface : IUserInterface
{
	public bool ConfirmPotDelete(string potName)
	{
		YesNoQuestion question = new($"Are you sure you want to delete pot '{potName}' together with its gems and snapshots?")
		{
			DefaultAnswer = YesNoAnswer.No
		};

		return question.ReadAnswer() == YesNoAnswer.Yes;
	}

	public bool ConfirmSnapshotsDelete(string potName, int snapshotCount)
	{
		YesNoQuestion question = new($"Are you sure you want to delete all {snapshotCount} snapshots of pot '{potName}'?")
		{
			DefaultAnswer = YesNoAnswer.No
		};

		return question.ReadAnswer() == YesNoAnswer.Yes;
	}
}