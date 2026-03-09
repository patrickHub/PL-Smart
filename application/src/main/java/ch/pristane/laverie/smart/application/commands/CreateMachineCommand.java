package ch.pristane.laverie.smart.application.commands;

import java.math.BigDecimal;

public record CreateMachineCommand(
        String name,
        BigDecimal pricePerCycle
) {
}