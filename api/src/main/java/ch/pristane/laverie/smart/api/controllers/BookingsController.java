package ch.pristane.laverie.smart.api.controllers;

import ch.pristane.laverie.smart.api.contracts.CreateBookingRequest;
import ch.pristane.laverie.smart.application.commands.CreateBookingCommand;
import ch.pristane.laverie.smart.application.dtos.BookingDto;
import ch.pristane.laverie.smart.application.services.BookingApplicationService;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.net.URI;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@RestController
@RequestMapping("/api/bookings")
public class BookingsController {

    private final BookingApplicationService bookingApplicationService;

    public BookingsController(BookingApplicationService bookingApplicationService) {
        this.bookingApplicationService = bookingApplicationService;
    }

    @GetMapping
    public ResponseEntity<List<BookingDto>> getAllBookings() {
        return ResponseEntity.ok(bookingApplicationService.getAllBookings());
    }

    @PostMapping
    public ResponseEntity<Map<String, UUID>> createBooking(@Valid @RequestBody CreateBookingRequest request) {
        UUID id = bookingApplicationService.createBooking(
                new CreateBookingCommand(
                        request.machineId(),
                        request.startTime(),
                        request.endTime(),
                        request.customerName()
                )
        );

        return ResponseEntity
                .created(URI.create("/api/bookings/" + id))
                .body(Map.of("id", id));
    }

    @PostMapping("/{id}/cancel")
    public ResponseEntity<Void> cancelBooking(@PathVariable UUID id) {
        bookingApplicationService.cancelBooking(id);
        return ResponseEntity.noContent().build();
    }

    @PostMapping("/{id}/complete")
    public ResponseEntity<Void> completeBooking(@PathVariable UUID id) {
        bookingApplicationService.completeBooking(id);
        return ResponseEntity.noContent().build();
    }
}