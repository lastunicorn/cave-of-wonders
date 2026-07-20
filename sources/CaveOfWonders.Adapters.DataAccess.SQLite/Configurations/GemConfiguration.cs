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

		entity.OwnsMany(
			x => x.Parameters,
			parameter =>
			{
				parameter.ToTable("GemParameters");
				parameter.WithOwner().HasForeignKey("GemId");

				// Same reasoning as PotLabel: a generated shadow "Id" doesn't work in a composite
				// key on SQLite. Keying on (GemId, Key) needs no generated value and assumes a gem
				// doesn't carry two parameters with the same key, which already held before this
				// was a database constraint.
				parameter.HasKey("GemId", nameof(GemParameter.Key));
			});
	}
}
