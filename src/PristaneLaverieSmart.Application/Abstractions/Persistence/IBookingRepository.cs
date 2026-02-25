using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Application.Abstractions.Persistence;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default);

    Task<bool> HasOverlapAsync(Guid machineId, DateTimeOffset start, DateTimeOffset end, CancellationToken ct = default);
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Booking booking, CancellationToken ct = default);
    Task<bool> HasActiveBookingNowAsync(Guid machineId, DateTimeOffset now, CancellationToken ct = default);
}