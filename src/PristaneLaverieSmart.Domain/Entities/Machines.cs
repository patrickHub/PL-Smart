using PristaneLaverieSmart.Domain.Enums;
namespace PristaneLaverieSmart.Domain.Entities;

public class Machine{
    public Guid Id {get; set;} = Guid.NewGuid();
    public string Name {get; set;} = string.Empty;
    public decimal PricePerCycle {get; set;}
    public MachineStatus Status {get; set;} = MachineStatus.Available;
}