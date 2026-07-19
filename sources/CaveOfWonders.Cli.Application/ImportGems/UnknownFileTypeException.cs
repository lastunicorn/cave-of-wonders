namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

public class UnknownFileTypeException : CaveOfWandersException
{
	private const string DefaultMessage = "Unknown file type '{0}'.";

	public UnknownFileTypeException(FileType fileType)
		: base(string.Format(DefaultMessage, fileType))
	{
	}
}