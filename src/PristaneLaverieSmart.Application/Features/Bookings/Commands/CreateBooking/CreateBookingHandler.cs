
using FluentValidation;
using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;

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

        var allMachines = await _repoMachine.GetAllAsync(ct);

        if(allMachines.All(m=>m.Id != request.MachineId))
        {
            throw new Common.Exceptions.NotFoundException("Machine not found");
        }

        Booking booking = new Booking
        {
            MachinedId = request.MachineId,
            CustomerName = request.CustomerName,
            StartTime = request.StartTime,
            EndTime = request.EndTime       
        };

        await _repoBooking.AddAsync(booking, ct);
        return booking.Id;
    }

}