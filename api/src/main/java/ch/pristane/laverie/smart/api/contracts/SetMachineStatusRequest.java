package ch.pristane.laverie.smart.api.contracts;

import ch.pristane.laverie.smart.domain.enums.MachineStatus;
import jakarta.validation.constraints.NotNull;

public record SetMachineStatusRequest(
        @NotNull(message = "Status is required.")
        MachineStatus status
) {
}
