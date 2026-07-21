namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.FileAccess.FileSystemTests.TestEnvironments;

/// <summary>
/// Back-door access to the sandbox directory backing a <see cref="FileSystemTestEnvironment"/> run, used by
/// Arrange to compute paths and seed pre-existing files/directories, and by Assert to inspect what was actually
/// written to disk without going through the SUT's own <c>IFileSystem</c> methods.
/// </summary>
public interface IFileSystemBackDoor
{
	string GetFullPath(string relativePath);

	bool FileExists(string relativePath);

	string ReadAllText(string relativePath);

	void WriteAllText(string relativePath, string content);

	void CreateDirectory(string relativeDirectoryPath);
}