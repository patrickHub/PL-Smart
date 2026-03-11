using MediatR;
using Microsoft.Extensions.Logging;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Events;

namespace PristaneLaverieSmart.Application.Features.Machines.EventHandlers;

public sealed class MachineStatusChangedHandler : INotificationHandler<MachineStatusChangedDomainEvent>
{
    private readonly ILogger<MachineStatusChangedHandler> _logger;
    private readonly IMachineStatusAuditRepository _auditRepo;

    public MachineStatusChangedHandler(ILogger<MachineStatusChangedHandler> logger, IMachineStatusAuditRepository auditRepo)
    {
        _logger = logger;
        _auditRepo = auditRepo;
    }

    public async Task Handle(MachineStatusChangedDomainEvent notification, CancellationToken ct)
    {
        var audit = new Domain.Entities.MachineStatusAudit
        {
            MachineId = notification.MachineId,
            OldStatus = notification.OldStatus,
            NewStatus = notification.NewStatus,
            OccurredOn = notification.OccuredOn
        };

        await _auditRepo.AddAsync(audit, ct);

         _logger.LogInformation(
            "Machine {MachineId} status changed: {OldStatus} -> {NewStatus}",
            notification.MachineId,
            notification.OldStatus,
            notification.NewStatus
        );

        //await Task.CompletedTask;
    }
}