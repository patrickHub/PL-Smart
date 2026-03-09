package ch.pristane.laverie.smart.api.contracts;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.time.OffsetDateTime;
import java.util.UUID;

public record CreateBookingRequest(
        @NotNull(message = "MachineId is required.")
        UUID machineId,

        @NotNull(message = "StartTime is required.")
        OffsetDateTime startTime,

        @NotNull(message = "EndTime is required.")
        OffsetDateTime endTime,

        @NotBlank(message = "CustomerName is required.")
        String customerName
) {
}