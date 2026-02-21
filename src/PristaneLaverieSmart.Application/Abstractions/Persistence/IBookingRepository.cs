using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Application.Abstractions.Persistence;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default);
}