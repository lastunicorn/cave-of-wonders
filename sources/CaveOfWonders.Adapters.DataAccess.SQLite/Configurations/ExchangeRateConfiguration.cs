using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;

internal class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
	public void Configure(EntityTypeBuilder<ExchangeRate> entity)
	{
		entity.Property<int>("Id").ValueGeneratedOnAdd();
		entity.HasKey("Id");

		entity
			.Property(x => x.CurrencyPair)
			.HasConversion(
				pair => pair.ToString(),
				value => string.IsNullOrEmpty(value) ? CurrencyPair.Empty : new CurrencyPair(value));

		entity
			.HasIndex(nameof(ExchangeRate.Date), nameof(ExchangeRate.CurrencyPair))
			.IsUnique();
	}
}
