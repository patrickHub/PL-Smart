package ch.pristane.laverie.smart.infrastructure.events;

import ch.pristane.laverie.smart.domain.events.BookingCancelledEvent;
import ch.pristane.laverie.smart.domain.events.BookingCompletedEvent;
import ch.pristane.laverie.smart.domain.events.BookingCreatedEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Component;

@Component
public class BookingEventLogger {

    private static final Logger log = LoggerFactory.getLogger(BookingEventLogger.class);

    @EventListener
    public void onBookingCreated(BookingCreatedEvent event) {
        System.out.println(">>> Booking created listener fired");
        log.info("Booking created: bookingId={}, machineId={}", event.bookingId(), event.machineId());
    }

    @EventListener
    public void onBookingCancelled(BookingCancelledEvent event) {
        log.info("Booking cancelled: bookingId={}, machineId={}", event.bookingId(), event.machineId());
    }

    @EventListener
    public void onBookingCompleted(BookingCompletedEvent event) {
        log.info("Booking completed: bookingId={}, machineId={}", event.bookingId(), event.machineId());
    }
}