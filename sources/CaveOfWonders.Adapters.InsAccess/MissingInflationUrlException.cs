namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

[Serializable]
internal class MissingInflationUrlException : Exception
{
    private const string DefaultMessage = "The URL for the inflation values was not provided in the appsettings.json file. Path: 'Ins.InflationPageUrl'.";

    public MissingInflationUrlException()
        : base(DefaultMessage)
    {
    }

    public MissingInflationUrlException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}