package ch.pristane.laverie.smart.domain.events;

import ch.pristane.laverie.smart.domain.enums.MachineStatus;

import java.time.OffsetDateTime;
import java.util.UUID;

public record MachineStatusChangedEvent(
        UUID machineId,
        MachineStatus oldStatus,
        MachineStatus newStatus,
        OffsetDateTime occurredOn
) {
}