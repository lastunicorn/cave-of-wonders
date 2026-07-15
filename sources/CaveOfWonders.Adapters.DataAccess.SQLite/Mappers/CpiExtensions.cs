using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Mappers;

internal static class CpiExtensions
{
	public static CpiEntity ToEntity(this Cpi cpi)
	{
		return new CpiEntity
		{
			Year = cpi.Year,
			Value = cpi.Value
		};
	}
}