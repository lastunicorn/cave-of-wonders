namespace DustInTheWind.CaveOfWonders.Ports.LogAccess;

public interface ILog
{
    void WriteSeparator();

    void WriteInfo(string text);

    void ExecuteInfo(string title, Action action);

    Task ExecuteInfo(string title, Func<Task> action);

    Task<T> ExecuteInfo<T>(string title, Func<Task<T>> action);
}