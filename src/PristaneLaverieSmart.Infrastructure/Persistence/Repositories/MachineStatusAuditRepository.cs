using Microsoft.EntityFrameworkCore;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;

namespace SmartLaundry.Infrastructure.Persistence.Repositories;

public sealed class MachineStatusAuditRepository : IMachineStatusAuditRepository
{
    private readonly PristaneLaverieSmartDbContext _db;
    public MachineStatusAuditRepository(PristaneLaverieSmartDbContext db) => _db = db;

    public void Add(MachineStatusAudit audit)
    {
        _db.MachineStatusAudits.Add(audit);
    }

    public async Task<IReadOnlyList<MachineStatusAudit>> GetByMachineIdAsync(Guid machineId, CancellationToken ct = default)
    {
        return await _db.MachineStatusAudits.AsNoTracking()
            .Where(a => a.MachineId == machineId)
            .OrderByDescending(a => a.OccurredOn)
            .ToListAsync(ct);
    }

    public async Task AddAsync(MachineStatusAudit audit, CancellationToken ct = default)
{
    _db.MachineStatusAudits.Add(audit);
    await _db.SaveChangesAsync(ct);
}
}