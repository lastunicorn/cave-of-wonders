namespace DustInTheWind.CaveOfWonders.Cli.Application.ExportCpi;

[Serializable]
public class OutputPathNotProvidedException : Exception
{
    private const string DefaultMessage = "The file path for exporting the CPI was not provided.";

    public OutputPathNotProvidedException()
        : base(DefaultMessage)
    {
    }
}