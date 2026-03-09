package ch.pristane.laverie.smart.domain.entities;

import java.math.BigDecimal;
import java.util.UUID;

import ch.pristane.laverie.smart.domain.MachineStatus;

public class Machine {
    private UUID id;
    private String name;
    private BigDecimal pricePerCycle;
    private MachineStatus status;

    public Machine() {
    }

    public Machine(UUID id, String name, BigDecimal pricePerCycle, MachineStatus status) {
        this.id = id;
        this.name = name;
        this.pricePerCycle = pricePerCycle;
        this.status = status;
    }

    public static Machine create(String name, BigDecimal pricePerCycle) {
        return new Machine(
                UUID.randomUUID(),
                name,
                pricePerCycle,
                MachineStatus.AVAILABLE
        );
    }

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public BigDecimal getPricePerCycle() {
        return pricePerCycle;
    }

    public MachineStatus getStatus() {
        return status;
    }

    public void setStatus(MachineStatus status) {
        this.status = status;
    }

}
