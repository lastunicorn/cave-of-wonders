using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.BcrAccess;

public interface IBcrService
{
	IAsyncEnumerable<Gem> GetGemsAsync(string filePath, CancellationToken cancellationToken);
}