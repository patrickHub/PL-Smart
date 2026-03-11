using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;

namespace PristaneLaverieSmart.Application.Features.Bookings.EventHandlers;

public sealed class BookingEndedHandler :
    INotificationHandler<BookingCancelledDomainEvent>,
    INotificationHandler<BookingCompletedDomainEvent>
{
    private readonly IMachineRepository _machines;
    private readonly IBookingRepository _bookings;

    public BookingEndedHandler(IMachineRepository machines, IBookingRepository bookings)
    {
        _machines = machines;
        _bookings = bookings;
    }

    public async Task Handle(BookingCancelledDomainEvent notification, CancellationToken ct)
        => await HandleEnded(notification.MachineId, ct);

    public async Task Handle(BookingCompletedDomainEvent notification, CancellationToken ct)
        => await HandleEnded(notification.MachineId, ct);

    private async Task HandleEnded(Guid machineId, CancellationToken ct)
    {
        var machine = await _machines.GetByIdAsync(machineId, ct);
        if (machine is null) return;

        if (machine.Status == MachineStatus.OutOfOrder) return;

        var now = DateTimeOffset.UtcNow;
        var active = await _bookings.HasActiveBookingNowAsync(machineId, now, ct);

        machine.SetStatus(active ? MachineStatus.Running : MachineStatus.Available);
        await _machines.UpdateAsync(machine, ct);
    }
}