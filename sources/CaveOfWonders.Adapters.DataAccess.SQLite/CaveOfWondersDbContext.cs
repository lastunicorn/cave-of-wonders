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
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Snapshots)
                .WithOne(e => e.Pot)
                .HasForeignKey(e => e.PotId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Labels)
                .WithOne(e => e.Pot)
                .HasForeignKey(e => e.PotId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PotSnapshotEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.PotId, e.Date }).IsUnique();
        });

        modelBuilder.Entity<PotLabelEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ExchangeRateEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.Date, e.CurrencyPair }).IsUnique();
        });

        modelBuilder.Entity<CpiEntity>(entity =>
        {
            entity.HasKey(e => e.Year);
        });

        modelBuilder.Entity<AverageWageEntity>(entity =>
        {
            entity.HasKey(e => e.Year);
        });

        modelBuilder.Entity<GemEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Pot)
                .WithMany()
                .HasForeignKey(e => e.PotId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Parameters)
                .WithOne(e => e.Gem)
                .HasForeignKey(e => e.GemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GemParameterEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
    }
}
