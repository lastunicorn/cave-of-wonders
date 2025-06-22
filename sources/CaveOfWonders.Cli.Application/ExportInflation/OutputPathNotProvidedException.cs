namespace DustInTheWind.CaveOfWonders.Cli.Application.ExportInflation;

[Serializable]
public class OutputPathNotProvidedException : Exception
{
    private const string DefaultMessage = "The file path for exporting the inflations was not provided.";

    public OutputPathNotProvidedException()
        : base(DefaultMessage)
    {
    }
}