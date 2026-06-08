namespace DustInTheWind.CaveOfWonders.Ports.FileAccess;

public interface IFileSystem
{
    Stream CreateFile(string path);
}
