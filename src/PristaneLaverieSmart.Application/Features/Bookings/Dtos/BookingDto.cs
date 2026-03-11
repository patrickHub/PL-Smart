using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Application.Features.Dtos;

public sealed record BookingDto (
    Guid Id,
    Guid MachineId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string CustomerName,
    BookingStatus Status
);