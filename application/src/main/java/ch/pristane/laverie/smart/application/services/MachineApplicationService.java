package ch.pristane.laverie.smart.application.services;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import ch.pristane.laverie.smart.application.commands.CreateMachineCommand;
import ch.pristane.laverie.smart.application.dtos.MachineDto;
import ch.pristane.laverie.smart.application.dtos.MachineStatusAuditDto;
import ch.pristane.laverie.smart.application.exceptions.BusinessRuleException;
import ch.pristane.laverie.smart.application.exceptions.NotFoundException;
import ch.pristane.laverie.smart.application.exceptions.ValidationException;
import ch.pristane.laverie.smart.application.ports.BookingRepository;
import ch.pristane.laverie.smart.application.ports.MachineRepository;
import ch.pristane.laverie.smart.application.ports.MachineStatusAuditRepository;
import ch.pristane.laverie.smart.domain.entities.Machine;
import ch.pristane.laverie.smart.domain.enums.BookingStatus;
import ch.pristane.laverie.smart.domain.enums.MachineStatus;
import ch.pristane.laverie.smart.domain.events.MachineStatusChangedEvent;

import org.springframework.context.ApplicationEventPublisher;
import org.springframework.stereotype.Service;

@Service
public class MachineApplicationService {

    private final MachineRepository machineRepository;
    private final BookingRepository bookingRepository;
    private final MachineStatusAuditRepository machineStatusAuditRepository;
    private final ApplicationEventPublisher eventPublisher;

    public MachineApplicationService(
            MachineRepository machineRepository,
            BookingRepository bookingRepository,
            MachineStatusAuditRepository machineStatusAuditRepository,
            ApplicationEventPublisher eventPublisher
    ) {
        this.machineRepository = machineRepository;
        this.bookingRepository = bookingRepository;
        this.machineStatusAuditRepository = machineStatusAuditRepository;
        this.eventPublisher = eventPublisher;
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

    public void setMachineStatus(UUID machineId, MachineStatus newStatus) {
        Machine machine = machineRepository.findById(machineId)
                .orElseThrow(() -> new NotFoundException("Machine not found."));

        boolean hasActiveBooking = bookingRepository.findAll().stream()
                .anyMatch(b ->
                        b.getMachineId().equals(machineId)
                                && b.getStatus() == BookingStatus.RECEIVED
                                && !b.getStartTime().isAfter(java.time.OffsetDateTime.now())
                                && b.getEndTime().isAfter(java.time.OffsetDateTime.now())
                );

        if (hasActiveBooking && newStatus == MachineStatus.OUT_OF_ORDER) {
            throw new BusinessRuleException("Cannot set machine to OutOfOrder while an active booking is running.");
        }

        if (hasActiveBooking && machine.getStatus() == MachineStatus.OUT_OF_ORDER && newStatus != MachineStatus.OUT_OF_ORDER) {
            throw new BusinessRuleException("Cannot change machine status while an active booking is running.");
        }

        MachineStatus oldStatus = machine.getStatus();
        machine.setStatus(newStatus);
        machineRepository.save(machine);

        if (oldStatus != newStatus) {
            eventPublisher.publishEvent(new MachineStatusChangedEvent(
                    machine.getId(),
                    oldStatus,
                    newStatus,
                    OffsetDateTime.now()
            ));
        }

    }

    public List<MachineStatusAuditDto> getMachineAudits(UUID machineId) {
        return machineStatusAuditRepository.findByMachineId(machineId)
                .stream()
                .map(a -> new ch.pristane.laverie.smart.application.dtos.MachineStatusAuditDto(
                        a.getId(),
                        a.getMachineId(),
                        a.getOldStatus(),
                        a.getNewStatus(),
                        a.getOccurredOn()
                ))
                .toList();
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