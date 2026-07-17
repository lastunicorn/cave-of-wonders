using DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Entities;
using Microsoft.EntityFrameworkCore;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;

public class CaveOfWondersDbContext : DbContext
{
	internal DbSet<PotEntity> Pots { get; set; }

	internal DbSet<PotSnapshotEntity> PotSnapshots { get; set; }

	internal DbSet<PotLabelEntity> PotLabels { get; set; }

	internal DbSet<ExchangeRateEntity> ExchangeRates { get; set; }

	internal DbSet<CpiEntity> Cpis { get; set; }

	internal DbSet<AverageWageEntity> AverageWages { get; set; }

	internal DbSet<GemEntity> Gems { get; set; }

	internal DbSet<GemParameterEntity> GemParameters { get; set; }

	internal ExchangeRateTracker ExchangeRateTracker { get; } = new();

	public CaveOfWondersDbContext(DbContextOptions<CaveOfWondersDbContext> options)
		: base(options)
	{
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		ExchangeRateTracker.PrepareChanges(ExchangeRates);
		int result = await base.SaveChangesAsync(cancellationToken);
		ExchangeRateTracker.CompleteChanges();

		return result;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaveOfWondersDbContext).Assembly);
	}
}