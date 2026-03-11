using FluentValidation;
using PristaneLaverieSmart.Application.Features.Machines.Commands;

public sealed class CreateMachineCommandValidator: AbstractValidator<CreateMachineCommand>
{
    public CreateMachineCommandValidator()
    {
        RuleFor(x=>x.Name)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x=>x.PricePerCycle)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);
    }
    
}