namespace DustInTheWind.CaveOfWonders.Cli.Application;

public class StorageInaccessibleException : CaveOfWandersException
{
    private const string DefaultMessage = "Storage is inaccessible.";

    public StorageInaccessibleException(Exception innerException)
        : base(DefaultMessage, innerException)
    {
    }
}