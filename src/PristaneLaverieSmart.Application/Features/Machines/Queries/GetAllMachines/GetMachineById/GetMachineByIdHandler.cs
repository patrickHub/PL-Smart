using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Machines.Dtos;

namespace PristaneLaverieSmart.Application.Features.Machines.Queries;

public sealed class GetMachineByIdHandler : IRequestHandler<GetMachineByIdQuery, MachineDto>
{
    private readonly IMachineRepository _repo;

    public GetMachineByIdHandler(IMachineRepository repo) => _repo = repo;

    public async Task<MachineDto> Handle(GetMachineByIdQuery request, CancellationToken ct)
    {
        var machine = await _repo.GetByIdAsync(request.Id, ct);
        if (machine is null)
            throw new NotFoundException("Machine not found.");

        return new MachineDto(machine.Id, machine.Name, machine.PricePerCycle, machine.Status);
    }
}