
using FluentValidation;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;

namespace PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandHandler
{
    private readonly IBookingRepository _repoBooking;
    private readonly IMachineRepository _repoMachine;
    private readonly IValidator<CreateBookingCommand> _validator;

    public CreateBookingCommandHandler(
        IBookingRepository repoBookings,
        IMachineRepository repoMachines,
        IValidator<CreateBookingCommand> validator)
    {
        _repoBooking = repoBookings;
        _repoMachine = repoMachines;
        _validator = validator;
    }

    public async Task<Guid> HandleAsync(CreateBookingCommand bookingCommand, CancellationToken ct = default)
    {

        await _validator.ValidateAndThrowAsync(bookingCommand, ct);

        var allMachines = await _repoMachine.GetAllAsync(ct);

        if(allMachines.All(m=>m.Id != bookingCommand.MachineId))
        {
            throw new Common.Exceptions.NotFoundException("Machine not found");
        }

        Booking booking = new Booking
        {
            MachinedId = bookingCommand.MachineId,
            CustomerName = bookingCommand.CustomerName,
            StartTime = bookingCommand.StartTime,
            EndTime = bookingCommand.EndTime       
        };

        await _repoBooking.AddAsync(booking, ct);
        return booking.Id;
    }

}