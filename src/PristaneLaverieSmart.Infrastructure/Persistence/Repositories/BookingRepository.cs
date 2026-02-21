using Microsoft.EntityFrameworkCore;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;
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
}