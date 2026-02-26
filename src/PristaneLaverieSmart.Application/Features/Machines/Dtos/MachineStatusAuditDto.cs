using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Application.Features.Machines.Dtos;

public sealed record MachineStatusAuditDto(
    Guid Id,
    Guid MachineId,
    MachineStatus OldStatus,
    MachineStatus NewStatus,
    DateTimeOffset OccurredOn
);