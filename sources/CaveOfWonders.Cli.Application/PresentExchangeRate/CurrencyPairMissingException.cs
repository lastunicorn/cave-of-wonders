namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

[Serializable]
public class CurrencyPairMissingException : Exception
{
	private const string DefaultMessage = "Currency pair value was not provided.";

	public CurrencyPairMissingException()
		: base(DefaultMessage)
	{
	}

	public CurrencyPairMissingException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}