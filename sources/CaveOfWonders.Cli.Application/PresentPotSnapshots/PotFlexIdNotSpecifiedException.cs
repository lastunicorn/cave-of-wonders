namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

public class PotFlexIdNotSpecifiedException : CaveOfWandersException
{
	private const string DefaultMessage = "The pot must be specified.";

	public PotFlexIdNotSpecifiedException()
		: base(DefaultMessage)
	{
	}
}
