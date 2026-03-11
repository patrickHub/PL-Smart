using PristaneLaverieSmart.Domain.Entities;
namespace PristaneLaverieSmart.Application.Abstractions.Persistence;

public interface IMachineRepository
{
    Task<IReadOnlyList<Machine>> GetAllAsync (CancellationToken ct = default);
    Task AddAsync(Machine machine, CancellationToken ct = default);
    Task<Machine?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Machine machine, CancellationToken ct = default);
}