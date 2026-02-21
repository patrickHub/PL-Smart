using Microsoft.EntityFrameworkCore;
using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Infrastructure.Persistence.DbContext;

public class PristaneLaverieSmartDbContext: Microsoft.EntityFrameworkCore.DbContext
{
    public PristaneLaverieSmartDbContext(DbContextOptions<PristaneLaverieSmartDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Machine>().HasData(
            new Machine
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Washer #1",
                PricePerCycle = 13.50m
            },
            new Machine
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Dryer #1",
                PricePerCycle = 1.50m
            }
        );
    }
}