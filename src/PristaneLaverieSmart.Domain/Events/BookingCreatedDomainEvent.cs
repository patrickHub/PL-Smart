using PristaneLaverieSmart.Domain.Common;

namespace PristaneLaverieSmart.Domain.Events;

public sealed record BookingCreatedDomainEvent(Guid BookingId, Guid MachineId, DateTimeOffset StartTime, DateTimeOffset EndTime) : IDomainEvent
{
    public DateTimeOffset OccuredOn {get;} = DateTimeOffset.UtcNow;
}