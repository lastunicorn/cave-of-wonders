namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.LogAccess.LogTests.TestEnvironments;

/// <summary>
/// Back-door access to the sandbox directory backing a <see cref="LogEnvironment"/> run, used by Arrange to
/// seed a pre-existing log file and by Assert to inspect what was actually written to disk without going through
/// the SUT's own <c>ILog</c> methods.
/// </summary>
public interface ILogBackDoor
{
	string GetLogFilePath();

	bool LogFileExists();

	List<string> ReadAllLines();

	void SeedLogFile(string content);
}