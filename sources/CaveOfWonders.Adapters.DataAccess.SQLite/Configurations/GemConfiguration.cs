using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class GemConfiguration : IEntityTypeConfiguration<Gem>
{
	public void Configure(EntityTypeBuilder<Gem> entity)
	{
		entity.HasKey(x => x.Id);

		entity.HasIndex("PotId", nameof(Gem.ExternalId));

		entity
			.HasOne(x => x.Pot)
			.WithMany()
			.HasForeignKey("PotId")
			.IsRequired()
			.OnDelete(DeleteBehavior.Restrict);

		entity
			.HasMany(x => x.Parameters)
			.WithOne(x => x.Gem)
			.HasForeignKey(x => x.GemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
