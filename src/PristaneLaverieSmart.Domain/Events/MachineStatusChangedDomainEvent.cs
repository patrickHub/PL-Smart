using PristaneLaverieSmart.Domain.Common;
using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Domain.Events;

public sealed record MachineStatusChangedDomainEvent(
    Guid MachineId,
    MachineStatus OldStatus,
    MachineStatus NewStatus
) : IDomainEvent
{
    public DateTimeOffset OccuredOn { get; } = DateTimeOffset.UtcNow;
}