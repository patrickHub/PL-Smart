using PristaneLaverieSmart.Application.Features.Machines.Dtos;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using MediatR;

namespace PristaneLaverieSmart.Application.Features.Machines.Queries;


public sealed class GetAllMachinesHandler: IRequestHandler<GetAllMachinesQuery, IReadOnlyList<MachineDto>>
{
    private readonly IMachineRepository _repo;

    public GetAllMachinesHandler(IMachineRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<MachineDto>> Handle(GetAllMachinesQuery request, CancellationToken ct = default)
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