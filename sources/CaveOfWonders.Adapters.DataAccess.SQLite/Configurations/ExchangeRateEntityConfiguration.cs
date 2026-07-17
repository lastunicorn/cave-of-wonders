using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class ExchangeRateEntityConfiguration : IEntityTypeConfiguration<ExchangeRateEntity>
{
	public void Configure(EntityTypeBuilder<ExchangeRateEntity> entity)
	{
		entity.HasKey(x => x.Id);
		
		entity
			.Property(x => x.Id)
			.ValueGeneratedOnAdd();
		
		entity
			.HasIndex(x => new
			{
				x.Date,
				x.CurrencyPair
			})
			.IsUnique();
	}
}


