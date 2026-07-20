using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Configurations;
using DustInTheWind.CaveOfWonders.Domain;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;

public class CaveOfWondersDbContext : DbContext
{
	public DbSet<Pot> Pots { get; set; }

	public DbSet<PotSnapshot> PotSnapshots { get; set; }

	public DbSet<ExchangeRate> ExchangeRates { get; set; }

	public DbSet<Cpi> Cpis { get; set; }

	public DbSet<AverageWage> AverageWages { get; set; }

	public DbSet<Gem> Gems { get; set; }

	public DbSet<GemParameter> GemParameters { get; set; }

	public CaveOfWondersDbContext(DbContextOptions<CaveOfWondersDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new AverageWageConfiguration());
		modelBuilder.ApplyConfiguration(new CpiConfiguration());
		modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());
		modelBuilder.ApplyConfiguration(new GemConfiguration());
		modelBuilder.ApplyConfiguration(new GemParameterConfiguration());
		modelBuilder.ApplyConfiguration(new PotConfiguration());
		modelBuilder.ApplyConfiguration(new PotSnapshotConfiguration());
	}
}
