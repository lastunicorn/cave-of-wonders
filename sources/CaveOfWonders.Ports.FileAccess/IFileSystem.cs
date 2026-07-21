namespace DustInTheWind.CaveOfWonders.Ports.FileAccess;

public interface IFileSystem
{
	Stream CreateFile(string path);

	IEnumerable<string> EnumerateFiles(string pathPattern);
}