using MediatR;
using PristaneLaverieSmart.Domain.Enums;

namespace PristaneLaverieSmart.Application.Features.Machines.Commands;

public sealed record SetMachineStatusCommand(Guid MachineId, MachineStatus Status) : IRequest<Unit>;