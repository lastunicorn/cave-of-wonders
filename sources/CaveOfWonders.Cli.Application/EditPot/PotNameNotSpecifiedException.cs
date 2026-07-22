namespace DustInTheWind.CaveOfWonders.Cli.Application.EditPot;

public class PotNameNotSpecifiedException : Exception
{
	private const string DefaultMessage = "Pot name not specified.";

	public PotNameNotSpecifiedException()
		: base(DefaultMessage)
	{
	}
}
