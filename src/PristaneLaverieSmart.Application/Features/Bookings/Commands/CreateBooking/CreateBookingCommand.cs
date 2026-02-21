namespace PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand(
    Guid MachineId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string CustomerName
);


