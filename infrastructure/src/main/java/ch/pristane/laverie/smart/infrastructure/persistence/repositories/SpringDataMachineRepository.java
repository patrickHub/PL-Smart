package ch.pristane.laverie.smart.infrastructure.persistence.repositories;

import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;

import ch.pristane.laverie.smart.infrastructure.persistence.entities.MachineJpaEntity;

public interface SpringDataMachineRepository extends JpaRepository<MachineJpaEntity, UUID> {
}
