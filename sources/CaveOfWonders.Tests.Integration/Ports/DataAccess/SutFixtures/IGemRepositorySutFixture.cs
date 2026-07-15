using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Utils;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;

/// <summary>
/// An <see cref="ISutFixture{T}"/> for <see cref="IGemRepository"/> that also allows seeding a <see cref="Pot"/>
/// directly into the same underlying storage backing the fixture's <see cref="IGemRepository"/> instance for that
/// phase. A Gem always belongs to a Pot - enforced as a foreign key by the SQLite adapter, and required for the
/// JSON adapter to re-attach the Gem's Pot reference on reload - so tests need a way to persist a matching Pot
/// before adding Gems.
/// </summary>
public interface IGemRepositorySutFixture : ISutFixture<IGemRepository>
{
	void SeedPot(Pot pot);
}