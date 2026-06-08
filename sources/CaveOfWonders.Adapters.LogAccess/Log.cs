using DustInTheWind.CaveOfWonders.Ports.LogAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.LogAccess;

public sealed class Log : ILog, IDisposable, IAsyncDisposable
{
    private readonly StreamWriter streamWriter;

    public Log()
    {
        string fileName = DateTime.Today.ToString("yyyy-MM-dd") + ".log";
        string logsDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Logs");

        if (!Directory.Exists(logsDirectoryPath))
            Directory.CreateDirectory(logsDirectoryPath);

        string filePath = Path.Combine(logsDirectoryPath, fileName);

        FileStreamOptions fileStreamOptions = new()
        {
            Mode = FileMode.Append,
            Access = FileAccess.Write
        };
        streamWriter = new StreamWriter(filePath, fileStreamOptions);
    }

    public void WriteSeparator()
    {
        WriteInfo(new string('-', 100));
    }

    public void WriteInfo(string text)
    {
        DateTime now = DateTime.Now;
        string message = $"[{now:yyyy-MM-dd HH:mm:ss.fff}] {text}";

        streamWriter.WriteLine(message);
    }

    public void Dispose()
    {
        streamWriter?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (streamWriter != null)
            await streamWriter.DisposeAsync();
    }

    public void ExecuteInfo(string title, Action action)
    {
        WriteSeparator();
        WriteInfo(title);

        try
        {
            action?.Invoke();
        }
        finally
        {
            WriteSeparator();
        }
    }

    public Task ExecuteInfo(string title, Func<Task> action)
    {
        WriteSeparator();
        WriteInfo(title);

        try
        {
            return action?.Invoke();
        }
        finally
        {
            WriteSeparator();
        }
    }

    public Task<T> ExecuteInfo<T>(string title, Func<Task<T>> action)
    {
        WriteSeparator();
        WriteInfo(title);

        try
        {
            return action?.Invoke();
        }
        finally
        {
            WriteSeparator();
        }
    }
}