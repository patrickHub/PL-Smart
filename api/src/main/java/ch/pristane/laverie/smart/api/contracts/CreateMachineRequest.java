package ch.pristane.laverie.smart.api.contracts;

import java.math.BigDecimal;

public record CreateMachineRequest(
        String name,
        BigDecimal pricePerCycle
) {
}