using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.DbMigration.DatabaseEndpoints;

/// <summary>
/// Wraps an <see cref="IUnitOfWork"/> together with whatever needs disposing
/// (an EF Core / LiteDB connection), regardless of which adapter is behind it.
/// </summary>
internal interface IDatabaseEndpoint : IDisposable
{
    IUnitOfWork UnitOfWork { get; }
}
