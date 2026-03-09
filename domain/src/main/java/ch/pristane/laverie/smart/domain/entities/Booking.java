package ch.pristane.laverie.smart.domain.entities;

import java.time.OffsetDateTime;
import java.util.UUID;

import ch.pristane.laverie.smart.domain.enums.BookingStatus;

public class Booking {
    private UUID id;
    private UUID machineId;
    private OffsetDateTime startTime;
    private OffsetDateTime endTime;
    private String customerName;
    private BookingStatus status;

    public Booking() {
    }

    public Booking(UUID id, UUID machineId, OffsetDateTime startTime, OffsetDateTime endTime, String customerName, BookingStatus status) {
        this.id = id;
        this.machineId = machineId;
        this.startTime = startTime;
        this.endTime = endTime;
        this.customerName = customerName;
        this.status = status;
    }

    public static Booking create(UUID machineId, OffsetDateTime startTime, OffsetDateTime endTime, String customerName) {
        return new Booking(
                UUID.randomUUID(),
                machineId,
                startTime,
                endTime,
                customerName,
                BookingStatus.RECEIVED
        );
    }

    public UUID getId() {
        return id;
    }

    public UUID getMachineId() {
        return machineId;
    }

    public OffsetDateTime getStartTime() {
        return startTime;
    }

    public OffsetDateTime getEndTime() {
        return endTime;
    }

    public String getCustomerName() {
        return customerName;
    }

    public BookingStatus getStatus() {
        return status;
    }

    
}
