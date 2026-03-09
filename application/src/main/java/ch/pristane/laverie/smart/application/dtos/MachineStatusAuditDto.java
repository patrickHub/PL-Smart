package ch.pristane.laverie.smart.application.dtos;

import ch.pristane.laverie.smart.domain.enums.MachineStatus;

import java.time.OffsetDateTime;
import java.util.UUID;

public record MachineStatusAuditDto(
        UUID id,
        UUID machineId,
        MachineStatus oldStatus,
        MachineStatus newStatus,
        OffsetDateTime occurredOn
) {
}