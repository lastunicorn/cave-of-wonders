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

	public bool ConfirmSnapshotsDelete(string potName, int snapshotCount, DateOnly? startDate = null, DateOnly? endDate = null)
	{
		string text = BuildSnapshotsDeleteQuestion(potName, snapshotCount, startDate, endDate);

		YesNoQuestion question = new(text)
		{
			DefaultAnswer = YesNoAnswer.No
		};

		return question.ReadAnswer() == YesNoAnswer.Yes;
	}

	private static string BuildSnapshotsDeleteQuestion(string potName, int snapshotCount, DateOnly? startDate, DateOnly? endDate)
	{
		if (startDate == null && endDate == null)
			return $"Are you sure you want to delete all {snapshotCount} snapshots of pot '{potName}'?";

		string interval = startDate != null && endDate != null
			? $"between {startDate.Value:yyyy-MM-dd} and {endDate.Value:yyyy-MM-dd}"
			: startDate != null
				? $"starting with {startDate.Value:yyyy-MM-dd}"
				: $"up to {endDate.Value:yyyy-MM-dd}";

		return $"Are you sure you want to delete the {snapshotCount} snapshots of pot '{potName}' {interval}?";
	}
}