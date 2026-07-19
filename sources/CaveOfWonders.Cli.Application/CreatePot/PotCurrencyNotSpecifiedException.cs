namespace DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;

public class PotCurrencyNotSpecifiedException : Exception
{
	private const string DefaultMessage = "Pot currency not specified.";

	public PotCurrencyNotSpecifiedException()
		: base(DefaultMessage)
	{
	}
}