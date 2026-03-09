package ch.pristane.laverie.smart.application.ports;

import ch.pristane.laverie.smart.domain.entities.Booking;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface BookingRepository {
    List<Booking> findAll();
    Optional<Booking> findById(UUID id);
    Booking save(Booking booking);
    boolean existsOverlappingBooking(UUID machineId, OffsetDateTime startTime, OffsetDateTime endTime);
}