namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class NoMatchingFilesFoundException : CaveOfWandersException
{
	private const string DefaultMessage = "No files found matching '{0}'.";

	public NoMatchingFilesFoundException(string filePathPattern)
		: base(string.Format(DefaultMessage, filePathPattern))
	{
	}
}
