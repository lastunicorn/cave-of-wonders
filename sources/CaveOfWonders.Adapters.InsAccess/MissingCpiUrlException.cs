namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

[Serializable]
internal class MissingCpiUrlException : Exception
{
	private const string DefaultMessage = "The URL for the CPI values was not provided in the appsettings.json file. Path: 'Ins.CpiPageUrl'.";

	public MissingCpiUrlException()
		: base(DefaultMessage)
	{
	}

	public MissingCpiUrlException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}