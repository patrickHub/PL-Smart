using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Application.Features.Machines.Commands;

public sealed class CreateMachineHandler
{
    IMachineRepository _repos;

    public CreateMachineHandler(IMachineRepository repos) => _repos = repos;

    public async Task<Guid> HandleAsync(CreateMachineCommand machineCommand, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(machineCommand.Name))
            throw new ArgumentException("Machine name is required");

        if(machineCommand.PricePerCycle <= 0)
            throw new ArgumentException("Price must be greater than zero");
            
        var machine = new Machine{
            Name = machineCommand.Name,
            PricePerCycle = machineCommand.PricePerCycle
        };

        await _repos.AddAsync(machine, ct);
        return machine.Id;
    }

}