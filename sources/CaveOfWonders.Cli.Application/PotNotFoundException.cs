namespace DustInTheWind.CaveOfWonders.Cli.Application;

public class PotNotFoundException : CaveOfWandersException
{
	private const string DefaultMessage = "No pot found with the id '{0}'.";

	public PotNotFoundException(string potId)
		: base(string.Format(DefaultMessage, potId))
	{
	}
}