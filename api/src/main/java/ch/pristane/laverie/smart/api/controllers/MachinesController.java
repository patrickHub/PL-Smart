package ch.pristane.laverie.smart.api.controllers;

import ch.pristane.laverie.smart.api.contracts.CreateMachineRequest;
import ch.pristane.laverie.smart.application.commands.CreateMachineCommand;
import ch.pristane.laverie.smart.application.dtos.MachineDto;
import ch.pristane.laverie.smart.application.services.MachineApplicationService;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.net.URI;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@RestController
@RequestMapping("/api/machines")
public class MachinesController {

    private final MachineApplicationService machineApplicationService;

    public MachinesController(MachineApplicationService machineApplicationService) {
        this.machineApplicationService = machineApplicationService;
    }

    @GetMapping
    public ResponseEntity<List<MachineDto>> getAllMachines() {
        return ResponseEntity.ok(machineApplicationService.getAllMachines());
    }

    @GetMapping("/{id}")
    public ResponseEntity<MachineDto> getMachineById(@PathVariable UUID id) {
        return ResponseEntity.ok(machineApplicationService.getMachineById(id));
    }

    @PostMapping
    public ResponseEntity<Map<String, UUID>> createMachine(@Valid @RequestBody CreateMachineRequest request) {
        UUID id = machineApplicationService.createMachine(
                new CreateMachineCommand(
                        request.name(),
                        request.pricePerCycle()
                )
        );

        return ResponseEntity
                .created(URI.create("/api/machines/" + id))
                .body(Map.of("id", id));
    }
}