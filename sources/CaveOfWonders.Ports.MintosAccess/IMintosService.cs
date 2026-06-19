using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Ports.MintosAccess;

public interface IMintosService
{
    IAsyncEnumerable<Gem> GetGemsAsync(string filePath, CancellationToken cancellationToken);
}