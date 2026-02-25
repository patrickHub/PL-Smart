using Microsoft.EntityFrameworkCore;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;

namespace PristaneLaverieSmart.Infrastructure.Persistence.Repository;


public sealed class BookingRepository : IBookingRepository
{
    private readonly PristaneLaverieSmartDbContext _db;

    public BookingRepository(PristaneLaverieSmartDbContext db) => _db = db;
    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Bookings.AsNoTracking().ToListAsync();
        
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public Task<bool> HasOverlapAsync(Guid machineId, DateTimeOffset start, DateTimeOffset end, CancellationToken ct = default)
    {
        return _db.Bookings.AsNoTracking()
            .Where(b => b.MachinedId == machineId && b.Status == BookingStatus.Received)
            .AnyAsync(b => b.StartTime < end && b.EndTime > start, ct);

    }

    public async Task<bool> HasActiveBookingNowAsync(Guid machineId, DateTimeOffset now, CancellationToken ct = default)
{
        return await _db.Bookings.AsNoTracking()
            .Where(b => b.MachinedId == machineId && b.Status == BookingStatus.Received)
            .AnyAsync(b => b.StartTime <= now && b.EndTime > now, ct);
    }

    public async Task UpdateAsync(Booking booking, CancellationToken ct = default)
    {
        _db.Bookings.Update(booking);
        await _db.SaveChangesAsync(ct);
    }
}