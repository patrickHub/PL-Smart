using PristaneLaverieSmart.Domain.Common;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;
namespace PristaneLaverieSmart.Domain.Entities;

public class Machine: Entity{
    public Guid Id {get; set;} = Guid.NewGuid();
    public string Name {get; set;} = string.Empty;
    public decimal PricePerCycle {get; set;}
    public MachineStatus Status {get; set;} = MachineStatus.Available;

    public void SetStatus(MachineStatus newStatus)
    {
        if (newStatus == Status) return;

        var old = Status;
        Status = newStatus;

        AddDomainEvent(new MachineStatusChangedDomainEvent(Id, old, newStatus));
    }
}