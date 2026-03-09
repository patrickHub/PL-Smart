package ch.pristane.laverie.smart.domain.events;

import java.time.OffsetDateTime;
import java.util.UUID;

public record BookingCreatedEvent(
        UUID bookingId,
        UUID machineId,
        OffsetDateTime startTime,
        OffsetDateTime endTime,
        OffsetDateTime occurredOn
) {
}