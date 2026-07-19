namespace DustInTheWind.CaveOfWonders.Cli.Application;

public class MultiplePotsException : CaveOfWandersException
{
	private const string DefaultMessage = "Multiple pots found with the id '{0}'.";

	public MultiplePotsException(string potId)
		: base(string.Format(DefaultMessage, potId))
	{
	}
}