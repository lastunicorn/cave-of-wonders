using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.QuanloopAccess;

public interface IQuanloopService
{
	IAsyncEnumerable<Gem> GetGemsAsync(string filePath, CancellationToken cancellationToken);
}
