using PristaneLaverieSmart.Application.Features.Machines.Dtos;
using PristaneLaverieSmart.Application.Abstractions.Persistence;

namespace PristaneLaverieSmart.Application.Features.Machines.Queries;


public sealed class GetAllMachinesHandler
{
    private readonly IMachineRepository _repo;

    public GetAllMachinesHandler(IMachineRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<MachineDto>> HandleAsync(CancellationToken ct = default)
    {
        var machines = await _repo.GetAllAsync(ct);
        return machines.Select(m => new MachineDto(
            m.Id,
            m.Name,
            m.PricePerCycle,
            m.Status
        )).ToList();
    }
}