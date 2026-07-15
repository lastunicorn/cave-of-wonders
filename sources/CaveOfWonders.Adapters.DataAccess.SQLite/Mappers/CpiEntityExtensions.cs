using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Mappers;

internal static class CpiEntityExtensions
{
	public static Cpi ToDomain(this CpiEntity entity)
	{
		return new Cpi
		{
			Year = entity.Year,
			Value = entity.Value
		};
	}
}