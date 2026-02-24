using MediatR;

namespace PristaneLaverieSmart.Application.Features.Machines.Commands;

public sealed record CreateMachineCommand(
    string Name,
    decimal PricePerCycle
): IRequest<Guid>;