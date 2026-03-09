package ch.pristane.laverie.smart.domain.entities;

import ch.pristane.laverie.smart.domain.enums.MachineStatus;

import java.time.OffsetDateTime;
import java.util.UUID;

public class MachineStatusAudit {
    private UUID id;
    private UUID machineId;
    private MachineStatus oldStatus;
    private MachineStatus newStatus;
    private OffsetDateTime occurredOn;

    public MachineStatusAudit() {
    }

    public MachineStatusAudit(UUID id, UUID machineId, MachineStatus oldStatus, MachineStatus newStatus, OffsetDateTime occurredOn) {
        this.id = id;
        this.machineId = machineId;
        this.oldStatus = oldStatus;
        this.newStatus = newStatus;
        this.occurredOn = occurredOn;
    }

    public static MachineStatusAudit create(UUID machineId, MachineStatus oldStatus, MachineStatus newStatus, OffsetDateTime occurredOn) {
        return new MachineStatusAudit(
                UUID.randomUUID(),
                machineId,
                oldStatus,
                newStatus,
                occurredOn
        );
    }

    public UUID getId() {
        return id;
    }

    public UUID getMachineId() {
        return machineId;
    }

    public MachineStatus getOldStatus() {
        return oldStatus;
    }

    public MachineStatus getNewStatus() {
        return newStatus;
    }

    public OffsetDateTime getOccurredOn() {
        return occurredOn;
    }
}