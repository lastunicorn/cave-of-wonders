using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.PeerBerryAccess;

public interface IPeerBerryService
{
	IAsyncEnumerable<Gem> GetGemsAsync(string filePath, CancellationToken cancellationToken);
}