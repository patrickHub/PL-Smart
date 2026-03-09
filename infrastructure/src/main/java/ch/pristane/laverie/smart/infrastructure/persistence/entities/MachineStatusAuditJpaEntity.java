package ch.pristane.laverie.smart.infrastructure.persistence.entities;

import ch.pristane.laverie.smart.domain.enums.MachineStatus;
import jakarta.persistence.*;

import java.time.OffsetDateTime;
import java.util.UUID;

@Entity
@Table(name = "machine_status_audits")
public class MachineStatusAuditJpaEntity {

    @Id
    @Column(name = "id", nullable = false, updatable = false)
    private UUID id;

    @Column(name = "machine_id", nullable = false)
    private UUID machineId;

    @Enumerated(EnumType.STRING)
    @Column(name = "old_status", nullable = false)
    private MachineStatus oldStatus;

    @Enumerated(EnumType.STRING)
    @Column(name = "new_status", nullable = false)
    private MachineStatus newStatus;

    @Column(name = "occurred_on", nullable = false)
    private OffsetDateTime occurredOn;

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public UUID getMachineId() {
        return machineId;
    }

    public void setMachineId(UUID machineId) {
        this.machineId = machineId;
    }

    public MachineStatus getOldStatus() {
        return oldStatus;
    }

    public void setOldStatus(MachineStatus oldStatus) {
        this.oldStatus = oldStatus;
    }

    public MachineStatus getNewStatus() {
        return newStatus;
    }

    public void setNewStatus(MachineStatus newStatus) {
        this.newStatus = newStatus;
    }

    public OffsetDateTime getOccurredOn() {
        return occurredOn;
    }

    public void setOccurredOn(OffsetDateTime occurredOn) {
        this.occurredOn = occurredOn;
    }
}