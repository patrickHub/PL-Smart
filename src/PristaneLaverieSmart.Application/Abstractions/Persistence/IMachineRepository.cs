using PristaneLaverieSmart.Domain.Entities;
namespace PristaneLaverieSmart.Application.Abstractions.Persistence;

public interface IMachineRepository
{
    Task<IReadOnlyList<Machine>> GetAllAsync (CancellationToken ct = default);
    Task AddAsync(Machine machine, CancellationToken ct = default);
}