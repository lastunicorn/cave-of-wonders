namespace DustInTheWind.CaveOfWonders.Cli.Application;

public class CaveOfWandersException : Exception
{
	private const string DefaultMessage = "An unknown exception happened.";

	public CaveOfWandersException(string message)
		: base(message)
	{
	}

	public CaveOfWandersException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public CaveOfWandersException(Exception innerException)
		: base(DefaultMessage, innerException)
	{
	}
}