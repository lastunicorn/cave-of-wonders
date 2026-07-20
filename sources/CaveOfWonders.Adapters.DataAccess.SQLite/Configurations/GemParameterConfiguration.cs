using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class GemParameterConfiguration : IEntityTypeConfiguration<GemParameter>
{
	public void Configure(EntityTypeBuilder<GemParameter> entity)
	{
		entity.HasKey(x => x.Id);

		entity
			.Property(x => x.Id)
			.ValueGeneratedOnAdd();
	}
}
