namespace DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

[Serializable]
public class SheetsException : Exception
{
    public SheetsException()
    {
    }

    public SheetsException(string message)
        : base(message)
    {
    }

    public SheetsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}