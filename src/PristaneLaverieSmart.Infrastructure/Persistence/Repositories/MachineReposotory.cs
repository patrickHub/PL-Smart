using Microsoft.EntityFrameworkCore;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;


namespace PristaneLaverieSmart.Infrastructure.Persistence.Repository;

public sealed class MachineRepository: IMachineRepository
{
    private readonly PristaneLaverieSmartDbContext _db;

    public MachineRepository(PristaneLaverieSmartDbContext db) => _db =  db;

    public async Task<IReadOnlyList<Machine>> GetAllAsync(CancellationToken ct = default)
        => await _db.Machines.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Machine machine, CancellationToken ct = default)
    {
        _db.Machines.Add(machine);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Machine?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
       return await _db.Machines.FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task UpdateAsync(Machine machine, CancellationToken ct = default)
    {
        _db.Machines.Update(machine);
        await _db.SaveChangesAsync(ct);
    }
}