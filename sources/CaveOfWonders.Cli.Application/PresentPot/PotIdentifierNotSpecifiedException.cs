namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

[Serializable]
public class PotIdentifierNotSpecifiedException : Exception
{
    private const string DefaultMessage = "Pot identifier must be specified.";

    public PotIdentifierNotSpecifiedException()
        : base(DefaultMessage)
    {
    }
}