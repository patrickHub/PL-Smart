using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Bookings.Commands;
using PristaneLaverieSmart.Domain.Enums;

namespace SmartLaundry.Application.Features.Bookings.Commands;

public sealed class CompleteBookingHandler : IRequestHandler<CompleteBookingCommand, Unit>
{
    private readonly IBookingRepository _repo;

    public CompleteBookingHandler(IBookingRepository repo) => _repo = repo;

    public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken ct)
    {
        var booking = await _repo.GetByIdAsync(request.BookingId, ct);
        if (booking is null)
            throw new NotFoundException("Booking not found.");

        if (booking.Status == BookingStatus.Completed)
            throw new BusinessRuleException("Booking is already completed.");

        if (booking.Status == BookingStatus.Cancelled)
            throw new BusinessRuleException("Cancelled bookings cannot be completed.");

        // Optional strict rule (enable if wanted):
        // if (DateTimeOffset.UtcNow < booking.EndTime)
        //     throw new BusinessRuleException("Booking cannot be completed before its EndTime.");

        booking.Status = BookingStatus.Completed;

        await _repo.UpdateAsync(booking, ct);
        return Unit.Value;
    }
}