using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class PotSnapshotConfiguration : IEntityTypeConfiguration<PotSnapshot>
{
	public void Configure(EntityTypeBuilder<PotSnapshot> entity)
	{
		entity.Property<int>("Id").ValueGeneratedOnAdd();
		entity.HasKey("Id");

		entity
			.HasIndex("PotId", nameof(PotSnapshot.Date))
			.IsUnique();
	}
}
