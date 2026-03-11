using PristaneLaverieSmart.Domain.Common;

namespace PristaneLaverieSmart.Domain.Events;

public sealed record BookingCompletedDomainEvent(Guid BookingId, Guid MachineId) : IDomainEvent
{
    public DateTimeOffset OccuredOn {get;} = DateTimeOffset.UtcNow;
}