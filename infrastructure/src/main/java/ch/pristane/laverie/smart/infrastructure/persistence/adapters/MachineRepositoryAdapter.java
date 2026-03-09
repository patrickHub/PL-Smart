package ch.pristane.laverie.smart.infrastructure.persistence.adapters;


import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.stereotype.Repository;

import ch.pristane.laverie.smart.application.ports.MachineRepository;
import ch.pristane.laverie.smart.domain.entities.Machine;
import ch.pristane.laverie.smart.infrastructure.persistence.entities.MachineJpaEntity;
import ch.pristane.laverie.smart.infrastructure.persistence.repositories.SpringDataMachineRepository;

@Repository
public class MachineRepositoryAdapter implements MachineRepository {

    private final SpringDataMachineRepository springDataMachineRepository;

    public MachineRepositoryAdapter(SpringDataMachineRepository springDataMachineRepository) {
        this.springDataMachineRepository = springDataMachineRepository;
    }

    @Override
    public List<Machine> findAll() {
        return springDataMachineRepository.findAll()
                .stream()
                .map(this::toDomain)
                .toList();
    }

    @Override
    public Optional<Machine> findById(UUID id) {
        return springDataMachineRepository.findById(id).map(this::toDomain);
    }

    @Override
    public Machine save(Machine machine) {
        MachineJpaEntity entity = toJpa(machine);
        MachineJpaEntity saved = springDataMachineRepository.save(entity);
        return toDomain(saved);
    }

    private Machine toDomain(MachineJpaEntity entity) {
        return new Machine(
                entity.getId(),
                entity.getName(),
                entity.getPricePerCycle(),
                entity.getStatus()
        );
    }

    private MachineJpaEntity toJpa(Machine machine) {
        MachineJpaEntity entity = new MachineJpaEntity();
        entity.setId(machine.getId());
        entity.setName(machine.getName());
        entity.setPricePerCycle(machine.getPricePerCycle());
        entity.setStatus(machine.getStatus());
        return entity;
    }
}