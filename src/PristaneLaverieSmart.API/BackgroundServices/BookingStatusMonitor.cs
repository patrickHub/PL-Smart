using Microsoft.EntityFrameworkCore;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;
using PristaneLaverieSmart.Domain.Enums;

namespace SmartLaundry.API.BackgroundServices;

public sealed class BookingStatusMonitor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingStatusMonitor> _logger;

    public BookingStatusMonitor(IServiceScopeFactory scopeFactory, ILogger<BookingStatusMonitor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PristaneLaverieSmartDbContext>();

                var now = DateTimeOffset.UtcNow;

                var activeBookings = await db.Bookings
                    .Where(b =>
                        b.Status == BookingStatus.Received &&
                        b.StartTime <= now &&
                        b.EndTime > now)
                    .ToListAsync(stoppingToken);

                foreach (var booking in activeBookings)
                {
                    var machine = await db.Machines.FirstOrDefaultAsync(
                        m => m.Id == booking.MachinedId,
                        stoppingToken);

                    if (machine is not null && machine.Status != MachineStatus.Running)
                    {
                        machine.Status = MachineStatus.Running;
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BookingStatusMonitor");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}