package ch.pristane.laverie.smart.infrastructure.persistence.repositories;

import ch.pristane.laverie.smart.domain.enums.BookingStatus;
import ch.pristane.laverie.smart.infrastructure.persistence.entities.BookingJpaEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.time.OffsetDateTime;
import java.util.UUID;

public interface SpringDataBookingRepository extends JpaRepository<BookingJpaEntity, UUID> {

    @Query("""
        select count(b) > 0
        from BookingJpaEntity b
        where b.machineId = :machineId
          and b.status = :status
          and b.startTime < :endTime
          and b.endTime > :startTime
    """)
    boolean existsOverlappingBooking(UUID machineId, OffsetDateTime startTime, OffsetDateTime endTime, BookingStatus status);
}