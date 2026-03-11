using System.ComponentModel;
using PristaneLaverieSmart.Domain.Common;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;

namespace PristaneLaverieSmart.Domain.Entities;

public class Booking: Entity
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public Guid MachinedId {get; set;}
    public DateTimeOffset StartTime {get; set;}
    public DateTimeOffset EndTime {get; set;}
    public string CustomerName {get; set;} = string.Empty;
    public BookingStatus Status {get; set;} = BookingStatus.Received;

    public static Booking Create(Guid machineId, DateTimeOffset start, DateTimeOffset end, string customerName)
    {
        var booking = new Booking
        {
            MachinedId = machineId,
            StartTime = start,
            EndTime = end,
            CustomerName = customerName,
            Status = BookingStatus.Received
        };

        booking.AddDomainEvent(new BookingCreatedDomainEvent(booking.Id, booking.MachinedId, booking.StartTime, booking.EndTime));
        return booking;
    }

    
    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
        AddDomainEvent(new BookingCancelledDomainEvent(Id, MachinedId));
    }
    public void Complete()
    {
        Status = BookingStatus.Completed;
        AddDomainEvent(new BookingCompletedDomainEvent(Id, MachinedId));
    }




}