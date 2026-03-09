package ch.pristane.laverie.smart.application.ports;


import java.util.List;
import java.util.Optional;
import java.util.UUID;

import ch.pristane.laverie.smart.domain.entities.Machine;

public interface MachineRepository {
    List<Machine> findAll();
    Optional<Machine> findById(UUID id);
    Machine save(Machine machine);
}