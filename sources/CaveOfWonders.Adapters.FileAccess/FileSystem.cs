using DustInTheWind.CaveOfWonders.Ports.FileAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.FileAccess;

public class FileSystem : IFileSystem
{
	public Stream CreateFile(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
			throw new ArgumentException("Path cannot be null or empty.", nameof(path));

		return File.Create(path);
	}

	public IEnumerable<string> EnumerateFiles(string pathPattern)
	{
		if (string.IsNullOrWhiteSpace(pathPattern))
			throw new ArgumentException("Path pattern cannot be null or empty.", nameof(pathPattern));

		string directoryPath = Path.GetDirectoryName(pathPattern);
		string searchPattern = Path.GetFileName(pathPattern);

		if (string.IsNullOrEmpty(directoryPath))
			directoryPath = ".";

		return Directory.EnumerateFiles(directoryPath, searchPattern, SearchOption.AllDirectories)
			.Order();
	}
}