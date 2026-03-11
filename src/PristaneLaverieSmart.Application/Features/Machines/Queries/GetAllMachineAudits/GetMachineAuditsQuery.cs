using MediatR;
using PristaneLaverieSmart.Application.Features.Machines.Dtos;

namespace PristaneLaverieSmart.Application.Features.Machines.Queries.GetAllMachineAudits;

public sealed record GetMachineAuditsQuery(Guid MachineId) : IRequest<IReadOnlyList<MachineStatusAuditDto>>;