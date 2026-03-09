package ch.pristane.laverie.smart.api.contracts;

import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.math.BigDecimal;

public record CreateMachineRequest(
        @NotBlank(message = "Machine name is required.")
        String name,

        @NotNull(message = "PricePerCycle is required.")
        @DecimalMin(value = "0.01", message = "Price must be greater than zero.")
        BigDecimal pricePerCycle
) {
}