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
}