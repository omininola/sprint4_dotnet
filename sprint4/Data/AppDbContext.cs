using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bike> Bikes { get; set; }
    public DbSet<Yard> Yards { get; set; }
    public DbSet<Subsidiary> Subsidiaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bike>()
            .HasOne(b => b.Yard)
            .WithMany(y => y.Bikes)
            .HasForeignKey(b => b.YardId);

        modelBuilder.Entity<Yard>()
            .HasOne(y => y.Subsidiary)
            .WithMany(s => s.Yards)
            .HasForeignKey(y => y.SubsidiaryId);
    }
}