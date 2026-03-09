package ch.pristane.laverie.smart.application.dtos;

import ch.pristane.laverie.smart.domain.enums.BookingStatus;

import java.time.OffsetDateTime;
import java.util.UUID;

public record BookingDto(
        UUID id,
        UUID machineId,
        OffsetDateTime startTime,
        OffsetDateTime endTime,
        String customerName,
        BookingStatus status
) {
}