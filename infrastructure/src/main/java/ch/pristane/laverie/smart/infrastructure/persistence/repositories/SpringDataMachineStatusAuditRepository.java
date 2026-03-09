package ch.pristane.laverie.smart.infrastructure.persistence.repositories;

import ch.pristane.laverie.smart.infrastructure.persistence.entities.MachineStatusAuditJpaEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.UUID;

public interface SpringDataMachineStatusAuditRepository extends JpaRepository<MachineStatusAuditJpaEntity, UUID> {
    List<MachineStatusAuditJpaEntity> findByMachineIdOrderByOccurredOnDesc(UUID machineId);
}