using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class GemParameterEntityConfiguration : IEntityTypeConfiguration<GemParameterEntity>
{
	public void Configure(EntityTypeBuilder<GemParameterEntity> entity)
	{
		entity.HasKey(x => x.Id);
		
		entity
			.Property(x => x.Id)
			.ValueGeneratedOnAdd();
	}
}


