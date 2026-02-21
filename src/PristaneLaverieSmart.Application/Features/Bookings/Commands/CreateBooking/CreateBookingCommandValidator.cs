using FluentValidation;
using PristaneLaverieSmart.Application.Features.Bookings.Commands;
using PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandValidator: AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x=>x.MachineId)
            .NotEmpty();
    
        RuleFor(x=>x.CustomerName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x=>x.EndTime)
            .GreaterThan(x=>x.StartTime)
            .WithMessage("EndTime must be after StartTime.");
        
        RuleFor(x=>x.StartTime)
            .Must(t=> t > DateTimeOffset.UtcNow.AddMinutes(-1))
            .WithMessage("StartTime must be in the future (UTC).");
    }
    
}