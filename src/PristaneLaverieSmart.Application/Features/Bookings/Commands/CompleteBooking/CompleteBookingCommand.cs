using MediatR;

namespace PristaneLaverieSmart.Application.Features.Bookings.Commands;

public sealed record CompleteBookingCommand(Guid BookingId) : IRequest<Unit>;