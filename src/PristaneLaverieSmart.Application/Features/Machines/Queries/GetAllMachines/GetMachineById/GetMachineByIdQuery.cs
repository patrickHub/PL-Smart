using MediatR;
using PristaneLaverieSmart.Application.Features.Machines.Dtos;

namespace SmartLaundry.Application.Features.Machines.Queries;

public sealed record GetMachineByIdQuery(Guid Id) : IRequest<MachineDto>;