using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWanders.Ports.MintosAccess;

public interface IMintosService
{
    IAsyncEnumerable<Gem> GetGemsAsync(string filePath, CancellationToken cancellationToken);
}