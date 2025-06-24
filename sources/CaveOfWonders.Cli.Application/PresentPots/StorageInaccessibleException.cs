namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

[Serializable]
public class StorageInaccessibleException : Exception
{
    private const string DefaultMessage = "The storage is inaccessible.";

    public StorageInaccessibleException(Exception innerException)
        : base(DefaultMessage, innerException)
    {
    }
}