using Microsoft.EntityFrameworkCore;
using ДЗ3.Models;

namespace ДЗ3.Data;

/// <summary>Контекст EF Core для БД автомобилей (Code First, SQLite).</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CarBrand> CarBrands => Set<CarBrand>();
    public DbSet<Car>      Cars      => Set<Car>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<CarBrand>(e =>
        {
            e.HasIndex(b => b.Name).IsUnique();
        });

        model.Entity<Car>(e =>
        {
            e.HasOne(c => c.Brand)
             .WithMany(b => b.Cars)
             .HasForeignKey(c => c.BrandId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
