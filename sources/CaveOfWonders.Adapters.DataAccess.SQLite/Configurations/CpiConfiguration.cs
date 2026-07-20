using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class CpiConfiguration : IEntityTypeConfiguration<Cpi>
{
	public void Configure(EntityTypeBuilder<Cpi> entity)
	{
		entity.HasKey(x => x.Year);
	}
}
