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
		modelBuilder.Entity<PotEntity>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity
				.HasMany(x => x.Snapshots)
				.WithOne(x => x.Pot)
				.HasForeignKey(x => x.PotId)
				.OnDelete(DeleteBehavior.Cascade);
			entity
				.HasMany(x => x.Labels)
				.WithOne(x => x.Pot)
				.HasForeignKey(x => x.PotId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<PotSnapshotEntity>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity
				.Property(x => x.Id)
				.ValueGeneratedOnAdd();
			entity
				.HasIndex(x => new
				{
					x.PotId,
					x.Date
				})
				.IsUnique();
		});

		modelBuilder.Entity<PotLabelEntity>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity
				.Property(x => x.Id)
				.ValueGeneratedOnAdd();
		});

		modelBuilder.Entity<ExchangeRateEntity>(entity =>
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
		});

		modelBuilder.Entity<CpiEntity>(entity =>
		{
			entity.HasKey(x => x.Year);
		});

		modelBuilder.Entity<AverageWageEntity>(entity =>
		{
			entity.HasKey(x => x.Year);
		});

		modelBuilder.Entity<GemEntity>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity
				.HasOne(x => x.Pot)
				.WithMany()
				.HasForeignKey(x => x.PotId)
				.OnDelete(DeleteBehavior.Restrict);
			entity
				.HasMany(x => x.Parameters)
				.WithOne(x => x.Gem)
				.HasForeignKey(x => x.GemId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<GemParameterEntity>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity
				.Property(x => x.Id)
				.ValueGeneratedOnAdd();
		});
	}
}