using PristaneLaverieSmart.Domain.Enums;
namespace PristaneLaverieSmart.Application.Features.Machines.Dtos;


public sealed record MachineDto (
    Guid Id,
    string Name,
    decimal PricePerCycle,
    MachineStatus Status
);