package ch.pristane.laverie.smart.domain.events;

import java.time.OffsetDateTime;
import java.util.UUID;

public record BookingCompletedEvent(
        UUID bookingId,
        UUID machineId,
        OffsetDateTime occurredOn
) {
}