using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Application.Features.Machines.Commands.SetMachineStatus;

public sealed class SetMachineStatusHandler : IRequestHandler<SetMachineStatusCommand, Unit>
{
    
    private readonly IMachineRepository _machines;
    private readonly IBookingRepository _bookings;

    public SetMachineStatusHandler(IMachineRepository machines, IBookingRepository bookings)
    {
        _machines = machines;
        _bookings = bookings;
    }
    public async Task<Unit> Handle(SetMachineStatusCommand request, CancellationToken ct = default)
    {
        var machine = await _machines.GetByIdAsync(request.MachineId, ct);
        if(machine is null)
        {
            throw new BusinessRuleException("Machine not found");
        }
         var now = DateTimeOffset.UtcNow;
        var hasActive = await _bookings.HasActiveBookingNowAsync(request.MachineId, now, ct);

        if (hasActive && request.Status == MachineStatus.OutOfOrder)
            throw new BusinessRuleException("Cannot set machine to OutOfOrder while an active booking is running.");

        if (hasActive && machine.Status == MachineStatus.OutOfOrder && request.Status != MachineStatus.OutOfOrder)
            throw new BusinessRuleException("Cannot change machine status while an active booking is running.");

        machine.SetStatus(request.Status);
        await _machines.UpdateAsync(machine, ct);

        return Unit.Value;
    }
}