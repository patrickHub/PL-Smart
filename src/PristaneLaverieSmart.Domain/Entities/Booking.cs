using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Domain.Entities;

public sealed class Booking
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public Guid MachinedId {get; set;}
    public DateTimeOffset StartTime {get; set;}
    public DateTimeOffset EndTime {get; set;}
    public string CustomerName {get; set;} = string.Empty;
    public BookingStatus Status {get; set;} = BookingStatus.Received;


}