using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class AverageWageConfiguration : IEntityTypeConfiguration<AverageWage>
{
	public void Configure(EntityTypeBuilder<AverageWage> entity)
	{
		entity.HasKey(x => x.Year);
		entity.Ignore(x => x.IsEmpty);
	}
}
