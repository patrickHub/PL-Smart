package ch.pristane.laverie.smart.application.services;

import java.util.List;
import java.util.Map;
import java.util.UUID;

import ch.pristane.laverie.smart.application.commands.CreateMachineCommand;
import ch.pristane.laverie.smart.application.dtos.MachineDto;
import ch.pristane.laverie.smart.application.exceptions.NotFoundException;
import ch.pristane.laverie.smart.application.exceptions.ValidationException;
import ch.pristane.laverie.smart.application.ports.MachineRepository;
import ch.pristane.laverie.smart.domain.entities.Machine;
import org.springframework.stereotype.Service;

@Service
public class MachineApplicationService {

    private final MachineRepository machineRepository;

    public MachineApplicationService(MachineRepository machineRepository) {
        this.machineRepository = machineRepository;
    }

    public List<MachineDto> getAllMachines() {
        return machineRepository.findAll()
                .stream()
                .map(this::toDto)
                .toList();
    }

    public MachineDto getMachineById(UUID id) {
        Machine machine = machineRepository.findById(id)
                .orElseThrow(() -> new NotFoundException("Machine not found."));
        return toDto(machine);
    }

    public UUID createMachine(CreateMachineCommand command) {
        validate(command);

        Machine machine = Machine.create(
                command.name().trim(),
                command.pricePerCycle()
        );

        Machine saved = machineRepository.save(machine);
        return saved.getId();
    }

    private void validate(CreateMachineCommand command) {
        if (command.name() == null || command.name().isBlank()) {
            throw new ValidationException(Map.of(
                    "name", new String[]{"Machine name is required."}
            ));
        }

        if (command.pricePerCycle() == null || command.pricePerCycle().signum() <= 0) {
            throw new ValidationException(Map.of(
                    "pricePerCycle", new String[]{"Price must be greater than zero."}
            ));
        }
    }

    private MachineDto toDto(Machine machine) {
        return new MachineDto(
                machine.getId(),
                machine.getName(),
                machine.getPricePerCycle(),
                machine.getStatus()
        );
    }
}