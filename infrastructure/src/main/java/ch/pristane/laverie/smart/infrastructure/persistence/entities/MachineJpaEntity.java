package ch.pristane.laverie.smart.infrastructure.persistence.entities;

import java.math.BigDecimal;
import java.util.UUID;

import ch.pristane.laverie.smart.domain.MachineStatus;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

@Entity
@Table(name = "machines")
public class MachineJpaEntity {

    @Id
    @Column(name = "id", nullable = false, updatable = false)
    private UUID id;

    @Column(name = "name", nullable = false)
    private String name;

    @Column(name = "price_per_cycle", nullable = false, precision = 10, scale = 2)
    private BigDecimal pricePerCycle;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false)
    private MachineStatus status;

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

    public void setName(String name) {
        this.name = name;
    }

    public void setPricePerCycle(BigDecimal pricePerCycle) {
        this.pricePerCycle = pricePerCycle;
    }

    public void setStatus(MachineStatus status) {
        this.status = status;
    }
}