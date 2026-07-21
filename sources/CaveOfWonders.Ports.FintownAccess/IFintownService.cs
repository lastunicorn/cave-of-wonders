using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.FintownAccess;

public interface IFintownService
{
	IAsyncEnumerable<Gem> GetGemsAsync(string filePath, CancellationToken cancellationToken);
}