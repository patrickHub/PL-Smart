using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PristaneLaverieSmart.Application.Common.Events;
using PristaneLaverieSmart.Domain.Common;
using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Infrastructure.Persistence.DbContext;

public class PristaneLaverieSmartDbContext: Microsoft.EntityFrameworkCore.DbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;
    public PristaneLaverieSmartDbContext(DbContextOptions<PristaneLaverieSmartDbContext> options, IDomainEventDispatcher? dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;   
    }
    public DbSet<MachineStatusAudit> MachineStatusAudits => Set<MachineStatusAudit>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         var dtoConverter = new ValueConverter<DateTimeOffset, DateTime>(
            v => v.UtcDateTime,
            v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc)));

        modelBuilder.Entity<Booking>()
            .Property(b => b.StartTime)
            .HasConversion(dtoConverter);
        modelBuilder.Entity<Booking>()
            .Property(b => b.EndTime)
            .HasConversion(dtoConverter);
         modelBuilder.Entity<MachineStatusAudit>()
            .Property(b => b.OccurredOn)
            .HasConversion(dtoConverter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Collect domain events BEFORE save (entities are tracked)
        var domainEntities = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var events = domainEntities.SelectMany(e => e.DomainEvents).ToList();

        // Clear first to prevent re-entrancy issues
        domainEntities.ForEach(e => e.ClearDomainEvents());

        var result = await base.SaveChangesAsync(ct);

        // Dispatch AFTER successful save
        if (_dispatcher is not null && events.Count > 0)
            await _dispatcher.DispatchAsync(events, ct);

        return result;
    }
}