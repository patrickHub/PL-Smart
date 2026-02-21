using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Features.Dtos;
using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Application.Queries.GetAllBookings;

public sealed class GetAllBookingsHandler
{
    private readonly IBookingRepository _repo;

    public GetAllBookingsHandler(IBookingRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<BookingDto>> HandleAsync(CancellationToken ct = default)
    {
        var bookings = await _repo.GetAllAsync(ct);
        return bookings.Select(b => 
            new BookingDto(
                b.Id,
                b.MachinedId,
                b.StartTime,
                b.EndTime,
                b.CustomerName,
                b.Status
            )
        ).ToList();
    }
}