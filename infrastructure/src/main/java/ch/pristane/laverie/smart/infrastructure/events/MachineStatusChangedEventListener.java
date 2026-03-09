package ch.pristane.laverie.smart.infrastructure.events;

import ch.pristane.laverie.smart.application.ports.MachineStatusAuditRepository;
import ch.pristane.laverie.smart.domain.entities.MachineStatusAudit;
import ch.pristane.laverie.smart.domain.events.MachineStatusChangedEvent;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Component;

@Component
public class MachineStatusChangedEventListener {

    private final MachineStatusAuditRepository auditRepository;

    public MachineStatusChangedEventListener(MachineStatusAuditRepository auditRepository) {
        this.auditRepository = auditRepository;
    }

    @EventListener
    public void handle(MachineStatusChangedEvent event) {
        MachineStatusAudit audit = MachineStatusAudit.create(
                event.machineId(),
                event.oldStatus(),
                event.newStatus(),
                event.occurredOn()
        );

        auditRepository.save(audit);
    }
}