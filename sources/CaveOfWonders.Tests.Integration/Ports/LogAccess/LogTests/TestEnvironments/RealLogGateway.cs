namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests.TestEnvironments;

internal sealed class RealLogGateway : ILogGateway
{
	private readonly string rootDirectoryPath;

	public RealLogGateway(string rootDirectoryPath)
	{
		this.rootDirectoryPath = rootDirectoryPath;
	}

	public string GetLogFilePath()
	{
		string fileName = DateTime.Today.ToString("yyyy-MM-dd") + ".log";
		return Path.Combine(rootDirectoryPath, "Logs", fileName);
	}

	public bool LogFileExists()
	{
		string logFilePath = GetLogFilePath();
		return File.Exists(logFilePath);
	}

	public List<string> ReadAllLines()
	{
		string logFilePath = GetLogFilePath();
		return File.ReadAllLines(logFilePath).ToList();
	}

	public void SeedLogFile(string content)
	{
		string logsDirectoryPath = Path.Combine(rootDirectoryPath, "Logs");
		Directory.CreateDirectory(logsDirectoryPath);

		string logFilePath = GetLogFilePath();
		File.WriteAllText(logFilePath, content);
	}
}
