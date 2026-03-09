package ch.pristane.laverie.smart.application.dtos;

import java.math.BigDecimal;
import java.util.UUID;

import ch.pristane.laverie.smart.domain.MachineStatus;

public record MachineDto(
        UUID id,
        String name,
        BigDecimal pricePerCycle,
        MachineStatus status
) {
}