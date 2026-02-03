namespace TwStock.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TwStock.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<FinancialStatement> FinancialStatements { get; set; }
    public DbSet<MarketData> MarketData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Stock Configuration
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Symbol).IsUnique();
            entity.Property(e => e.Symbol).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        // FinancialStatement Configuration
        modelBuilder.Entity<FinancialStatement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Stock)
                  .WithMany(s => s.FinancialStatements)
                  .HasForeignKey(e => e.StockId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Initial Index for fast lookup
            entity.HasIndex(e => new { e.StockId, e.Year, e.Quarter }).IsUnique();
        });

        // MarketData Configuration
        modelBuilder.Entity<MarketData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Stock)
                  .WithMany(s => s.MarketData)
                  .HasForeignKey(e => e.StockId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.StockId, e.Date }).IsUnique();
        });
    }
}
