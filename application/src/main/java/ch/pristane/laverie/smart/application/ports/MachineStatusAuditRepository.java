package ch.pristane.laverie.smart.application.ports;

import ch.pristane.laverie.smart.domain.entities.MachineStatusAudit;

import java.util.List;
import java.util.UUID;

public interface MachineStatusAuditRepository {
    MachineStatusAudit save(MachineStatusAudit audit);
    List<MachineStatusAudit> findByMachineId(UUID machineId);
}