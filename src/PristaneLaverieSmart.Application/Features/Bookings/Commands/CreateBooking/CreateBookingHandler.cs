
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingHandler: IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _repoBooking;
    private readonly IMachineRepository _repoMachine;

    public CreateBookingHandler(
        IBookingRepository repoBookings,
        IMachineRepository repoMachines)
    {
        _repoBooking = repoBookings;
        _repoMachine = repoMachines;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken ct = default)
    {

        var machine = await _repoMachine.GetByIdAsync(request.MachineId);
        Console.WriteLine(machine);

        if(machine is null)
        {
            throw new Common.Exceptions.BusinessRuleException("Machine not found");
        }
        if(machine.Status == MachineStatus.OutOfOrder)
        {
            throw new Common.Exceptions.BusinessRuleException("Cannot create a booking: machine is OutOfOrder.");
        }
        bool hasOverlapping = await _repoBooking.HasOverlapAsync(machine.Id, request.StartTime, request.EndTime, ct);
        if (hasOverlapping)
        {
            throw new Common.Exceptions.BusinessRuleException("Cannot create a booking: time slot overlaps with an existing booking.");
        }

        var booking = Booking.Create(request.MachineId, request.StartTime, request.EndTime, request.CustomerName);
        
        await _repoBooking.AddAsync(booking, ct);
        return booking.Id;
    }

}