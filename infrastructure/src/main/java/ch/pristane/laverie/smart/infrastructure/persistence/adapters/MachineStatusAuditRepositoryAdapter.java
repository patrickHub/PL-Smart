package ch.pristane.laverie.smart.infrastructure.persistence.adapters;

import ch.pristane.laverie.smart.application.ports.MachineStatusAuditRepository;
import ch.pristane.laverie.smart.domain.entities.MachineStatusAudit;
import ch.pristane.laverie.smart.infrastructure.persistence.entities.MachineStatusAuditJpaEntity;
import ch.pristane.laverie.smart.infrastructure.persistence.repositories.SpringDataMachineStatusAuditRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public class MachineStatusAuditRepositoryAdapter implements MachineStatusAuditRepository {

    private final SpringDataMachineStatusAuditRepository repository;

    public MachineStatusAuditRepositoryAdapter(SpringDataMachineStatusAuditRepository repository) {
        this.repository = repository;
    }

    @Override
    public MachineStatusAudit save(MachineStatusAudit audit) {
        MachineStatusAuditJpaEntity entity = toJpa(audit);
        MachineStatusAuditJpaEntity saved = repository.save(entity);
        return toDomain(saved);
    }

    @Override
    public List<MachineStatusAudit> findByMachineId(UUID machineId) {
        return repository.findByMachineIdOrderByOccurredOnDesc(machineId)
                .stream()
                .map(this::toDomain)
                .toList();
    }

    private MachineStatusAuditJpaEntity toJpa(MachineStatusAudit audit) {
        MachineStatusAuditJpaEntity entity = new MachineStatusAuditJpaEntity();
        entity.setId(audit.getId());
        entity.setMachineId(audit.getMachineId());
        entity.setOldStatus(audit.getOldStatus());
        entity.setNewStatus(audit.getNewStatus());
        entity.setOccurredOn(audit.getOccurredOn());
        return entity;
    }

    private MachineStatusAudit toDomain(MachineStatusAuditJpaEntity entity) {
        return new MachineStatusAudit(
                entity.getId(),
                entity.getMachineId(),
                entity.getOldStatus(),
                entity.getNewStatus(),
                entity.getOccurredOn()
        );
    }
}