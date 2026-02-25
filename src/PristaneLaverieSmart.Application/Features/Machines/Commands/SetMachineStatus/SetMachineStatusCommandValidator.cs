using FluentValidation;
using PristaneLaverieSmart.Application.Features.Machines.Commands;

namespace SmartLaundry.Application.Features.Machines.Commands;

public sealed class SetMachineStatusCommandValidator : AbstractValidator<SetMachineStatusCommand>
{
    public SetMachineStatusCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}