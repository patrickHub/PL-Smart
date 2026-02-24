using MediatR;
using PristaneLaverieSmart.Application.Features.Machines.Dtos;

namespace PristaneLaverieSmart.Application.Features.Machines.Queries;

public sealed record GetAllMachinesQuery(): IRequest<IReadOnlyList<MachineDto>>;