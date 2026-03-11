using MediatR;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Features.Machines.Dtos;

namespace PristaneLaverieSmart.Application.Features.Machines.Queries.GetAllMachineAudits;

public sealed class GetMachineAuditsHandler : IRequestHandler<GetMachineAuditsQuery, IReadOnlyList<MachineStatusAuditDto>>
{
    private readonly IMachineStatusAuditRepository _repo;

    public GetMachineAuditsHandler(IMachineStatusAuditRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<MachineStatusAuditDto>> Handle(GetMachineAuditsQuery request, CancellationToken ct)
    {
        var audits = await _repo.GetByMachineIdAsync(request.MachineId, ct);

        return audits.Select(a => new MachineStatusAuditDto(
            a.Id,
            a.MachineId,
            a.OldStatus,
            a.NewStatus,
            a.OccurredOn
        )).ToList();
    }
}