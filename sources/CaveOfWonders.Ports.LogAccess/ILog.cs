namespace DustInTheWind.CaveOfWonders.Ports.LogAccess;

public interface ILog
{
    void WriteInfo(string text);

    Task<T> ExecuteInfo<T>(string title, Func<Task<T>> action);
}