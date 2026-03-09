package ch.pristane.laverie.smart.application.commands;

import java.time.OffsetDateTime;
import java.util.UUID;

public record CreateBookingCommand(
        UUID machineId,
        OffsetDateTime startTime,
        OffsetDateTime endTime,
        String customerName
) {
}