namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.FileAccess.FileSystemTests.TestEnvironments;

internal sealed class FileSystemBackDoor : IFileSystemBackDoor
{
	private readonly string rootDirectoryPath;

	public FileSystemBackDoor(string rootDirectoryPath)
	{
		this.rootDirectoryPath = rootDirectoryPath;
	}

	public string GetFullPath(string relativePath)
	{
		return Path.Combine(rootDirectoryPath, relativePath);
	}

	public bool FileExists(string relativePath)
	{
		return File.Exists(GetFullPath(relativePath));
	}

	public string ReadAllText(string relativePath)
	{
		return File.ReadAllText(GetFullPath(relativePath));
	}

	public void WriteAllText(string relativePath, string content)
	{
		File.WriteAllText(GetFullPath(relativePath), content);
	}

	public void CreateDirectory(string relativeDirectoryPath)
	{
		Directory.CreateDirectory(GetFullPath(relativeDirectoryPath));
	}
}
