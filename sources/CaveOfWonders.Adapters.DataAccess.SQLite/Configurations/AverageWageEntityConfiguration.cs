using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class AverageWageEntityConfiguration : IEntityTypeConfiguration<AverageWageEntity>
{
	public void Configure(EntityTypeBuilder<AverageWageEntity> entity)
	{
		entity.HasKey(x => x.Year);
	}
}


