using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class CpiEntityConfiguration : IEntityTypeConfiguration<CpiEntity>
{
	public void Configure(EntityTypeBuilder<CpiEntity> entity)
	{
		entity.HasKey(x => x.Year);
	}
}


