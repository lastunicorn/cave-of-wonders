using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class GemEntityConfiguration : IEntityTypeConfiguration<GemEntity>
{
	public void Configure(EntityTypeBuilder<GemEntity> entity)
	{
		entity.HasIndex(x => new { x.PotId, x.ExternalId });

		entity.HasKey(x => x.Id);

		entity
			.HasOne(x => x.Pot)
			.WithMany()
			.HasForeignKey(x => x.PotId)
			.OnDelete(DeleteBehavior.Restrict);

		entity
			.HasMany(x => x.Parameters)
			.WithOne(x => x.Gem)
			.HasForeignKey(x => x.GemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}