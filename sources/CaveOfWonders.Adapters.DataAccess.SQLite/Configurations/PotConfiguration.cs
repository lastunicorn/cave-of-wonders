using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class PotConfiguration : IEntityTypeConfiguration<Pot>
{
	public void Configure(EntityTypeBuilder<Pot> entity)
	{
		entity.HasKey(x => x.Id);

		entity
			.HasMany(x => x.Snapshots)
			.WithOne(x => x.Pot)
			.HasForeignKey("PotId")
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		entity.OwnsMany(
			x => x.Labels,
			label =>
			{
				label.ToTable("PotLabels");
				label.WithOwner().HasForeignKey("PotId");

				// A generated shadow "Id" doesn't work here: SQLite only auto-generates a value
				// for a single-column INTEGER PRIMARY KEY (its rowid alias), not for a column
				// that's part of a composite key. Using (PotId, Label) as the key sidesteps that
				// entirely and needs no generated value — duplicate labels on the same pot
				// wouldn't be meaningful anyway.
				label.HasKey("PotId", nameof(PotLabel.Label));
			});
	}
}