using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Application.Features.Bookings.Commands.CancelBooking;


public sealed class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IBookingRepository _repos;

    public CancelBookingHandler(IBookingRepository repos) => _repos = repos;
    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var booking = await _repos.GetByIdAsync(request.BookingId, ct);
        if(booking is null)
        {
            throw new NotFoundException("Booking not found");
        }
        if(booking.Status == BookingStatus.Cancelled)
        {
            throw new BusinessRuleException("Booking already cancelled.");
        }
        if(booking.Status == BookingStatus.Completed)
        {
            throw new BusinessRuleException("Completed Booking can not be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;

        await _repos.UpdateAsync(booking, ct);
        return Unit.Value;
        
    }
}