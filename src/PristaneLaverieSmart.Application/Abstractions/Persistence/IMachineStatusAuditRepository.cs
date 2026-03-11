using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Application.Abstractions.Persistence;

public interface IMachineStatusAuditRepository
{
    void Add(MachineStatusAudit audit);
    Task<IReadOnlyList<MachineStatusAudit>> GetByMachineIdAsync(Guid machineId, CancellationToken ct = default);
    Task AddAsync(MachineStatusAudit audit, CancellationToken ct = default);
}