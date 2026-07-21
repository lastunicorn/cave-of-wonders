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
}