namespace DustInTheWind.CaveOfWonders.Cli.Application;

public class DataStorageException : CaveOfWandersException
{
    private const string DefaultMessage = "Data storage error.";

    public DataStorageException(Exception innerException)
        : base(DefaultMessage, innerException)
    {
    }
}