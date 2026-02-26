using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;

namespace PristaneLaverieSmart.Application.Features.Bookings.EventHandlers;

public sealed class BookingCreatedHandler : INotificationHandler<BookingCreatedDomainEvent>
{
    private readonly IMachineRepository _machines;
    private readonly IBookingRepository _bookings;

    public BookingCreatedHandler(IMachineRepository machines, IBookingRepository bookings)
    {
        _machines = machines;
        _bookings = bookings;
    }

    public async Task Handle(BookingCreatedDomainEvent notification, CancellationToken ct)
    {
        var machine = await _machines.GetByIdAsync(notification.MachineId, ct);
        if (machine is null) return;

        var now = DateTimeOffset.UtcNow;

        // If booking is active immediately, set Running (but don't override OutOfOrder)
        if (machine.Status != MachineStatus.OutOfOrder)
        {
            var active = await _bookings.HasActiveBookingNowAsync(notification.MachineId, now, ct);
            if (active && machine.Status != MachineStatus.Running)
            {
                //machine.Status = MachineStatus.Running;
                machine.SetStatus(MachineStatus.Running);
                await _machines.UpdateAsync(machine, ct);
            }
        }
    }
}