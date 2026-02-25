using MediatR;

namespace PristaneLaverieSmart.Application.Features.Bookings.Commands;

public sealed record CancelBookingCommand(Guid BookingId) : IRequest<Unit>;