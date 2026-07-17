using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class PotEntityConfiguration : IEntityTypeConfiguration<PotEntity>
{
	public void Configure(EntityTypeBuilder<PotEntity> entity)
	{
		entity.HasKey(x => x.Id);
		
		entity
			.HasMany(x => x.Snapshots)
			.WithOne(x => x.Pot)
			.HasForeignKey(x => x.PotId)
			.OnDelete(DeleteBehavior.Cascade);
		
		entity
			.HasMany(x => x.Labels)
			.WithOne(x => x.Pot)
			.HasForeignKey(x => x.PotId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}


