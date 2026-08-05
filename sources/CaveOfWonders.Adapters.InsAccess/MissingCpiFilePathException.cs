namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

[Serializable]
internal class MissingCpiFilePathException : Exception
{
	private const string DefaultMessage = "The path of the file containing the CPI values was not provided. Parameter name: 'FilePath'.";

	public MissingCpiFilePathException()
		: base(DefaultMessage)
	{
	}

	public MissingCpiFilePathException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
