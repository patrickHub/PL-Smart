using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Domain.Entities;

public class MachineStatusAudit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MachineId { get; set; }
    public MachineStatus OldStatus { get; set; }
    public MachineStatus NewStatus { get; set; }
    public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
}