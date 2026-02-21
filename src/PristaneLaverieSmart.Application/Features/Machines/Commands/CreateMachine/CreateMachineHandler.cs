using FluentValidation;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
// using PristaneLaverieSmart.Application.Common.Validation;
using PristaneLaverieSmart.Domain.Entities;


namespace PristaneLaverieSmart.Application.Features.Machines.Commands;

public sealed class CreateMachineHandler
{
    private readonly IMachineRepository _repos;
    private readonly IValidator<CreateMachineCommand> _validator;

    public CreateMachineHandler(IMachineRepository repos, IValidator<CreateMachineCommand> validator)
    {
         _repos = repos;
         _validator = validator;
        
    }

    public async Task<Guid> HandleAsync(CreateMachineCommand machineCommand, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(machineCommand, ct);
            
        var machine = new Machine{
            Name = machineCommand.Name,
            PricePerCycle = machineCommand.PricePerCycle
        };

        await _repos.AddAsync(machine, ct);
        return machine.Id;
    }

}