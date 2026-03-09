package ch.pristane.laverie.smart.infrastructure.persistence.adapters;

import ch.pristane.laverie.smart.application.ports.BookingRepository;
import ch.pristane.laverie.smart.domain.entities.Booking;
import ch.pristane.laverie.smart.domain.enums.BookingStatus;
import ch.pristane.laverie.smart.infrastructure.persistence.entities.BookingJpaEntity;
import ch.pristane.laverie.smart.infrastructure.persistence.repositories.SpringDataBookingRepository;
import org.springframework.stereotype.Repository;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public class BookingRepositoryAdapter implements BookingRepository {

    private final SpringDataBookingRepository springDataBookingRepository;

    public BookingRepositoryAdapter(SpringDataBookingRepository springDataBookingRepository) {
        this.springDataBookingRepository = springDataBookingRepository;
    }

    @Override
    public List<Booking> findAll() {
        return springDataBookingRepository.findAll()
                .stream()
                .map(this::toDomain)
                .toList();
    }

    @Override
    public Optional<Booking> findById(UUID id) {
        return springDataBookingRepository.findById(id).map(this::toDomain);
    }

    @Override
    public Booking save(Booking booking) {
        BookingJpaEntity saved = springDataBookingRepository.save(toJpa(booking));
        return toDomain(saved);
    }

    @Override
    public boolean existsOverlappingBooking(UUID machineId, OffsetDateTime startTime, OffsetDateTime endTime) {
        return springDataBookingRepository.existsOverlappingBooking(
                machineId,
                startTime,
                endTime,
                BookingStatus.RECEIVED
        );
    }

    private Booking toDomain(BookingJpaEntity entity) {
        return new Booking(
                entity.getId(),
                entity.getMachineId(),
                entity.getStartTime(),
                entity.getEndTime(),
                entity.getCustomerName(),
                entity.getStatus()
        );
    }

    private BookingJpaEntity toJpa(Booking booking) {
        BookingJpaEntity entity = new BookingJpaEntity();
        entity.setId(booking.getId());
        entity.setMachineId(booking.getMachineId());
        entity.setStartTime(booking.getStartTime());
        entity.setEndTime(booking.getEndTime());
        entity.setCustomerName(booking.getCustomerName());
        entity.setStatus(booking.getStatus());
        return entity;
    }
}