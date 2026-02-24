using FluentValidation;
using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
// using PristaneLaverieSmart.Application.Common.Validation;
using PristaneLaverieSmart.Domain.Entities;


namespace PristaneLaverieSmart.Application.Features.Machines.Commands;

public sealed class CreateMachineHandler: IRequestHandler<CreateMachineCommand, Guid>
{
    private readonly IMachineRepository _repos;

    public CreateMachineHandler(IMachineRepository repos) => _repos = repos;

    public async Task<Guid> Handle(CreateMachineCommand request, CancellationToken ct = default)
    {
            
        var machine = new Machine{
            Name = request.Name.Trim(),
            PricePerCycle = request.PricePerCycle
        };

        await _repos.AddAsync(machine, ct);
        return machine.Id;
    }

}