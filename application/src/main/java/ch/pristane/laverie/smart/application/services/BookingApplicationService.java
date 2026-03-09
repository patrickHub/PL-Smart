package ch.pristane.laverie.smart.application.services;

import ch.pristane.laverie.smart.application.commands.CreateBookingCommand;
import ch.pristane.laverie.smart.application.dtos.BookingDto;
import ch.pristane.laverie.smart.application.exceptions.BusinessRuleException;
import ch.pristane.laverie.smart.application.exceptions.NotFoundException;
import ch.pristane.laverie.smart.application.exceptions.ValidationException;
import ch.pristane.laverie.smart.application.ports.BookingRepository;
import ch.pristane.laverie.smart.application.ports.MachineRepository;
import ch.pristane.laverie.smart.domain.entities.Booking;
import ch.pristane.laverie.smart.domain.entities.Machine;
import ch.pristane.laverie.smart.domain.enums.BookingStatus;
import ch.pristane.laverie.smart.domain.enums.MachineStatus;
import org.springframework.stereotype.Service;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@Service
public class BookingApplicationService {

    private final BookingRepository bookingRepository;
    private final MachineRepository machineRepository;

    public BookingApplicationService(BookingRepository bookingRepository, MachineRepository machineRepository) {
        this.bookingRepository = bookingRepository;
        this.machineRepository = machineRepository;
    }

    public List<BookingDto> getAllBookings() {
        return bookingRepository.findAll()
                .stream()
                .map(this::toDto)
                .toList();
    }

    public UUID createBooking(CreateBookingCommand command) {
        validate(command);

        Machine machine = machineRepository.findById(command.machineId())
                .orElseThrow(() -> new NotFoundException("Machine not found."));

        if (machine.getStatus() == MachineStatus.OUT_OF_ORDER) {
            throw new BusinessRuleException("Cannot create a booking: machine is OutOfOrder.");
        }

        boolean overlap = bookingRepository.existsOverlappingBooking(
                command.machineId(),
                command.startTime(),
                command.endTime()
        );

        if (overlap) {
            throw new BusinessRuleException("Cannot create a booking: time slot overlaps with an existing booking.");
        }

        Booking booking = Booking.create(
                command.machineId(),
                command.startTime(),
                command.endTime(),
                command.customerName().trim()
        );

        Booking saved = bookingRepository.save(booking);
        return saved.getId();
    }

    public void cancelBooking(UUID bookingId) {
        Booking booking = bookingRepository.findById(bookingId)
                .orElseThrow(() -> new NotFoundException("Booking not found."));

        try {
            booking.cancel();
        } catch (IllegalStateException ex) {
            throw new BusinessRuleException(ex.getMessage());
        }

        bookingRepository.save(booking);
        refreshMachineStatus(booking.getMachineId());
    }

    public void completeBooking(UUID bookingId) {
        Booking booking = bookingRepository.findById(bookingId)
                .orElseThrow(() -> new NotFoundException("Booking not found."));

        try {
            booking.complete();
        } catch (IllegalStateException ex) {
            throw new BusinessRuleException(ex.getMessage());
        }

        bookingRepository.save(booking);
        refreshMachineStatus(booking.getMachineId());
    }

    private void refreshMachineStatus(UUID machineId) {
        Machine machine = machineRepository.findById(machineId)
                .orElseThrow(() -> new NotFoundException("Machine not found."));

        if (machine.getStatus() == MachineStatus.OUT_OF_ORDER) {
            return;
        }

        boolean hasActiveBooking = bookingRepository.findAll().stream()
                .anyMatch(b ->
                        b.getMachineId().equals(machineId)
                                && b.getStatus() == BookingStatus.RECEIVED
                                && !b.getStartTime().isAfter(OffsetDateTime.now())
                                && b.getEndTime().isAfter(OffsetDateTime.now())
                );

        machine.setStatus(hasActiveBooking ? MachineStatus.RUNNING : MachineStatus.AVAILABLE);
        machineRepository.save(machine);
    }

    private void validate(CreateBookingCommand command) {
        if (command.machineId() == null) {
            throw new ValidationException(Map.of(
                    "machineId", new String[]{"MachineId is required."}
            ));
        }

        if (command.customerName() == null || command.customerName().isBlank()) {
            throw new ValidationException(Map.of(
                    "customerName", new String[]{"CustomerName is required."}
            ));
        }

        if (command.startTime() == null) {
            throw new ValidationException(Map.of(
                    "startTime", new String[]{"StartTime is required."}
            ));
        }

        if (command.endTime() == null) {
            throw new ValidationException(Map.of(
                    "endTime", new String[]{"EndTime is required."}
            ));
        }

        if (command.startTime() != null && command.endTime() != null
                && !command.endTime().isAfter(command.startTime())) {
            throw new ValidationException(Map.of(
                    "endTime", new String[]{"EndTime must be after StartTime."}
            ));
        }

        if (command.startTime() != null && command.startTime().isBefore(OffsetDateTime.now().minusMinutes(1))) {
            throw new ValidationException(Map.of(
                    "startTime", new String[]{"StartTime must be now or in the future."}
            ));
        }
    }

    private BookingDto toDto(Booking booking) {
        return new BookingDto(
                booking.getId(),
                booking.getMachineId(),
                booking.getStartTime(),
                booking.getEndTime(),
                booking.getCustomerName(),
                booking.getStatus()
        );
    }
}