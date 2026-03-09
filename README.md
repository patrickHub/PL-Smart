
# Java (Spring Boot) vs .NET (ASP.NET Core) Backend Implementation
### Case Study: Pristane Laverie Smart API

This repository contains **two implementations of the same backend system** built with different technology stacks.

Branch structure:

- **main branch → .NET implementation**
  - ASP.NET Core
  - Entity Framework Core
  - MediatR
  - FluentValidation

- **java-springboot branch → Java implementation**
  - Spring Boot
  - Spring Data JPA / Hibernate
  - Spring Events
  - Jakarta Bean Validation

The goal of this project is to demonstrate that **the same backend architecture and business rules can be implemented consistently across different ecosystems**.

Both versions implement the same:

- REST API contract
- business rules
- domain model
- integration tests
- production hardening practices

This document compares both implementations.

---

# 1. Project Overview

The system manages:

- Washing machines
- Machine status lifecycle
- Booking reservations
- Booking cancellation and completion
- Machine status audit trail
- Overlap prevention
- Business rule validation

Both implementations expose a REST API with identical endpoints.

Example endpoints:

```
GET    /api/machines
GET    /api/machines/{id}
POST   /api/machines

POST   /api/bookings
GET    /api/bookings

POST   /api/bookings/{id}/cancel
POST   /api/bookings/{id}/complete

POST   /api/machines/{id}/status
GET    /api/machines/{id}/audits
```

---

# 2. Architecture Overview

Both implementations use layered architecture inspired by Clean Architecture.

## .NET Architecture

```
API
 ├── Controllers
 ├── Middleware
 └── Program.cs

Application
 ├── Commands
 ├── Queries
 ├── Handlers (MediatR)
 ├── DTOs
 └── Interfaces (Ports)

Domain
 ├── Entities
 ├── Enums
 └── Domain Events

Infrastructure
 ├── EF Core DbContext
 ├── Repositories
 ├── Event dispatching
 └── Persistence
```

## Java Architecture

```
api
 ├── controllers
 ├── filters
 └── error handlers

application
 ├── services
 ├── commands
 ├── dtos
 └── ports

domain
 ├── entities
 ├── enums
 └── events

infrastructure
 ├── persistence (JPA)
 ├── repositories
 ├── event listeners
 └── migrations (Flyway)
```

---

# 3. Dependency Injection

## .NET

Uses **Microsoft Dependency Injection.**

Example:

```csharp
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
```

Dependencies injected via constructor.

```csharp
public MachineApplicationService(IMachineRepository repo)
```

## Java

Uses **Spring Dependency Injection.**

```java
@Service
public class MachineApplicationService {
}
```

Dependencies injected via constructor.

```java
public MachineApplicationService(MachineRepository machineRepository)
```

---

# 4. Domain Entities
Entities in both implementations represent the same domain objects.

## Machine

Attributes:

- id
- name
- pricePerCycle
- status

Status values:

```
Available
Running
OutOfOrder
```

## Booking

Attributes:

```
id
machineId
startTime
endTime
customerName
status
```

Status values:

```
Reserved
Cancelled
Completed
```

---

# 5. Enum Serialization
Ensuring both APIs return the **same enum strings** required custom handling.

## .NET
Configured globally:

```csharp
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
```
Enums automatically serialized as:
```
Available
Running
OutOfOrder
```

## Java

Using Jackson configuration with `@JsonValue` and `@JsonCreator`:

```java
@JsonValue
public String getValue()
```
This ensures the API returns the same enum strings as the .NET version.

---

# 6. Persistence Layer

## .NET

Persistence stack:

```
Entity Framework Core
SQLite
```

Example DbContext:

```csharp
public class PristaneLaverieSmartDbContext : DbContext
```
Repositories implemented using EF Core.

## Java

Persistence stack:

```
Spring Data JPA
Hibernate
SQLite
Flyway migrations
```

Example repository:

```java
public interface SpringDataMachineRepository extends JpaRepository<MachineJpaEntity, UUID>
```

---

# 7. Domain Events
Both implementations support **domain events**, but the mechanisms differ.

## .NET
Domain events are stored on entities and dispatched after persistence.

```
Entity
   ↓
DomainEvent list
   ↓
SaveChangesAsync
   ↓
DomainEventDispatcher
   ↓
MediatR handlers
```

Example event:

```
MachineStatusChangedDomainEvent
```
Handled by:

```csharp
MachineStatusChangedHandler
```

## Java
Spring's **ApplicationEventPublisher** is used.

```
Application Service
   ↓
ApplicationEventPublisher.publishEvent()
   ↓
Spring Event Bus
   ↓
@EventListener
```

Example:

```java
eventPublisher.publishEvent(new MachineStatusChangedEvent(...));
```

Handled by:
```java
@EventListener
public void handle(MachineStatusChangedEvent event)
```

---

# 8. Audit Trail Implementation

Machine status changes are recorded in an audit table.

Table:
```
machine_status_audits
```

Columns:

```
id
machine_id
old_status
new_status
occurred_on
```

## .NET
Audit written by **MediatR event handler.**

```
MachineStatusChangedDomainEvent
     ↓
MachineStatusChangedHandler
     ↓
MachineStatusAuditRepository
```

## Java
Audit written by **Spring event listener**.

```
MachineStatusChangedEvent
     ↓
MachineStatusChangedEventListener
     ↓
MachineStatusAuditRepositoryAdapter
```

---

# 9. Validation

## .NET

Using **FluentValidation.**

```csharp
RuleFor(x => x.Name).NotEmpty();
```
Validation is executed via a **MediatR pipeline behavior.**

## Java

Uses **Bean Validation (Jakarta Validation).**

Example
```java
@NotBlank
String name;
```
Triggered automatically by:
```java
@valid
```
in controllers.

---

# 10. Error Handling

Both implementations return **ProblemDetails-style responses.**

Example response:
```json
{
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "name": ["Machine name is required."]
  }
}
```

## .NET

```
GlobalExceptionMiddleware
```

## Java

```
@RestControllerAdvice
```

---

# 11. Logging and Correlation IDs
Both implementations support request tracing.

## .NET

```
Serilog
CorrelationIdMiddleware
```
Example log:
```
[correlationId=abc123] Booking created
```

## Java

```
SLF4J
Logback
CorrelationIdFilter
```
Correlation ID stored in MDC.

---

# 12. Integration Testing
Both implementations include full integration tests.

Tests verify:

- booking creation
- overlap prevention
- machine status rules
- audit trail generation

## .NET

```
xUnit
WebApplicationFactory
FluentAssertions
```

## Java

```
SpringBootTest
MockMvc
JUnit 5
```

---

# 13. Developer Experience

| Feature | .NET | Java |
|------|------|------|
| Build tool | dotnet CLI | Maven |
| Dependency injection | Built-in | Spring |
| ORM | EF Core | Hibernate |
| Event handling | MediatR | Spring Events |
| Validation | FluentValidation | Jakarta Validation |
| Testing | xUnit | JUnit |

---

# 14. Key Observations

### .NET advantages

Tests verify:

- Very clean **CQRS support via MediatR**
- FluentValidation integration is powerful
- Minimal boilerplate
- Strong typing with records

### Java advantages

- Mature **Spring ecosystem**
- Strong ORM support
- Highly configurable framework
- Large enterprise adoption

---

# 15. Conclusion

Both ecosystems allow building robust backend architectures.

The major difference is **framework philosophy**:

- .NET provides a **lighter framework with explicit libraries**
- Spring Boot provides a **more integrated framework**

However, **architectural patterns remain the same**:
- Clean Architecture
- Domain-driven design
- Event-driven components
- Layered persistence

---

# 16. Side-by-Side Code Equivalents
This section highlights equivalent implementations in both ecosystems for the same backend use cases.

## 16.1 Create Machine Use Case

### .NET - MediatR Command + Handler

```csharp
public sealed record CreateMachineCommand(
    string Name,
    decimal PricePerCycle
) : IRequest<Guid>;

public sealed class CreateMachineHandler : IRequestHandler<CreateMachineCommand, Guid>
{
    private readonly IMachineRepository _repo;

    public CreateMachineHandler(IMachineRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateMachineCommand request, CancellationToken ct)
    {
        var machine = new Machine
        {
            Name = request.Name.Trim(),
            PricePerCycle = request.PricePerCycle,
            Status = MachineStatus.Available
        };

        await _repo.AddAsync(machine, ct);
        return machine.Id;
    }
}
```

### Java - Application Service

```java
public record CreateMachineCommand(
        String name,
        BigDecimal pricePerCycle
) {
}

@Service
public class MachineApplicationService {

    private final MachineRepository machineRepository;

    public MachineApplicationService(MachineRepository machineRepository) {
        this.machineRepository = machineRepository;
    }

    public UUID createMachine(CreateMachineCommand command) {
        Machine machine = Machine.create(
                command.name().trim(),
                command.pricePerCycle()
        );

        Machine saved = machineRepository.save(machine);
        return saved.getId();
    }
}
```
### Comparison

- .NET uses a dedicated command + handler model via MediatR.
- Java uses a simpler application service pattern.
- Both approaches isolate business logic from controllers.
- The .NET version is more CQRS-oriented by default.
- The Java version is more idiomatic for Spring projects unless a custom mediator layer is added.

---

## 16.2 Get Machine by Id
### .NET - Query + Handler

```csharp
public sealed record GetMachineByIdQuery(Guid Id) : IRequest<MachineDto>;

public sealed class GetMachineByIdHandler : IRequestHandler<GetMachineByIdQuery, MachineDto>
{
    private readonly IMachineRepository _repo;

    public GetMachineByIdHandler(IMachineRepository repo)
    {
        _repo = repo;
    }

    public async Task<MachineDto> Handle(GetMachineByIdQuery request, CancellationToken ct)
    {
        var machine = await _repo.GetByIdAsync(request.Id, ct)
                     ?? throw new NotFoundException("Machine not found.");

        return new MachineDto(
            machine.Id,
            machine.Name,
            machine.PricePerCycle,
            machine.Status
        );
    }
}
```
### Java - Application Service Method
```csharp
public MachineDto getMachineById(UUID id) {
    Machine machine = machineRepository.findById(id)
            .orElseThrow(() -> new NotFoundException("Machine not found."));

    return new MachineDto(
            machine.getId(),
            machine.getName(),
            machine.getPricePerCycle(),
            machine.getStatus()
    );
}
```
### Comparison

- The .NET version separates reads into explicit query objects.
- The Java version keeps reads as service methods.
- Both are valid; the .NET version scales better for large CQRS-heavy systems.
- The Java version is more concise for smaller modules.

## 16.3 Repository Abstraction
### .NET - Repository Interface

```csharp
public interface IMachineRepository
{
    Task<IReadOnlyList<Machine>> GetAllAsync(CancellationToken ct = default);
    Task<Machine?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Machine machine, CancellationToken ct = default);
    Task UpdateAsync(Machine machine, CancellationToken ct = default);
}
```

### Java - Repository Port
```java
public interface MachineRepository {
    List<Machine> findAll();
    Optional<Machine> findById(UUID id);
    Machine save(Machine machine);
}
```

### Comparison
- Both implementations define repositories in the application layer as ports/interfaces.
- .NET usually separates AddAsync and UpdateAsync.
- Java/Spring Data often uses a single save() for both insert and update.
- Both preserve Clean Architecture boundaries.

## 16.4 Persistence Implementation
### .NET - EF Core Repository
```csharp
public sealed class MachineRepository : IMachineRepository
{
    private readonly PristaneLaverieSmartDbContext _db;

    public MachineRepository(PristaneLaverieSmartDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Machine>> GetAllAsync(CancellationToken ct = default)
        => await _db.Machines.AsNoTracking().ToListAsync(ct);

    public async Task<Machine?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Machines.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task AddAsync(Machine machine, CancellationToken ct = default)
    {
        _db.Machines.Add(machine);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Machine machine, CancellationToken ct = default)
    {
        _db.Machines.Update(machine);
        await _db.SaveChangesAsync(ct);
    }
}
```

### Java - JPA Repository Adapter

```csharp
@Repository
public class MachineRepositoryAdapter implements MachineRepository {

    private final SpringDataMachineRepository springDataMachineRepository;

    public MachineRepositoryAdapter(SpringDataMachineRepository springDataMachineRepository) {
        this.springDataMachineRepository = springDataMachineRepository;
    }

    @Override
    public List<Machine> findAll() {
        return springDataMachineRepository.findAll()
                .stream()
                .map(this::toDomain)
                .toList();
    }

    @Override
    public Optional<Machine> findById(UUID id) {
        return springDataMachineRepository.findById(id).map(this::toDomain);
    }

    @Override
    public Machine save(Machine machine) {
        MachineJpaEntity saved = springDataMachineRepository.save(toJpa(machine));
        return toDomain(saved);
    }

    private Machine toDomain(MachineJpaEntity entity) {
        return new Machine(
                entity.getId(),
                entity.getName(),
                entity.getPricePerCycle(),
                entity.getStatus()
        );
    }

    private MachineJpaEntity toJpa(Machine machine) {
        MachineJpaEntity entity = new MachineJpaEntity();
        entity.setId(machine.getId());
        entity.setName(machine.getName());
        entity.setPricePerCycle(machine.getPricePerCycle());
        entity.setStatus(machine.getStatus());
        return entity;
    }
}
```

### Comparison

- EF Core works directly with domain-like entities more naturally.
- JPA/Hibernate often encourages a separate persistence entity layer, especially in a stricter Clean Architecture setup.
- The Java adapter is a more explicit hexagonal port/adapter implementation.
- The .NET repository is shorter because EF Core mapping can be more direct.

## 16.5 Validation
### .NET - FluentValidation
```csharp
public sealed class CreateMachineCommandValidator : AbstractValidator<CreateMachineCommand>
{
    public CreateMachineCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PricePerCycle)
            .GreaterThan(0);
    }
}
```

### Java - Bean Validation + Service Rules
```java
public record CreateMachineRequest(
        @NotBlank(message = "Machine name is required.")
        String name,

        @NotNull(message = "PricePerCycle is required.")
        @DecimalMin(value = "0.01", message = "Price must be greater than zero.")
        BigDecimal pricePerCycle
) {
}
```

### Comparison

- **.NET** validation is centralized in FluentValidation and can run in a MediatR pipeline.
- **Java** validation is split between:
  - request-level validation via Jakarta annotations
  - business validation inside services
- FluentValidation gives more expressive rule composition.
- Jakarta Validation is more declarative for input contracts.

## 16.6 Global Error Handling
### .NET - Middleware
```csharp
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Resource not found",
                status = 404,
                detail = ex.Message
            });
        }
    }
}
```

### Java - `@RestControllerAdvice`
```java
@RestControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(NotFoundException.class)
    public ResponseEntity<ApiErrorResponse> handleNotFoundException(
            NotFoundException ex,
            HttpServletRequest request
    ) {
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(
                new ApiErrorResponse(
                        "Resource not found",
                        404,
                        ex.getMessage(),
                        null,
                        request.getRequestURI()
                )
        );
    }
}
```

### Comparison
- **.NET** uses middleware in the HTTP pipeline.
- **Java** uses Spring MVC exception advice.
- Both achieve centralized error shaping.
- Middleware feels lower-level and more pipeline-oriented.
- `@ControllerAdvice` feels more framework-integrated and declarative.

## 16.7 Domain Events
### .NET - Domain Event + MediatR Handler
```csharp
public sealed record MachineStatusChangedDomainEvent(
    Guid MachineId,
    MachineStatus OldStatus,
    MachineStatus NewStatus
) : IDomainEvent;

public sealed class MachineStatusChangedHandler : INotificationHandler<MachineStatusChangedDomainEvent>
{
    private readonly IMachineStatusAuditRepository _auditRepo;

    public MachineStatusChangedHandler(IMachineStatusAuditRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public async Task Handle(MachineStatusChangedDomainEvent notification, CancellationToken ct)
    {
        await _auditRepo.AddAsync(new MachineStatusAudit
        {
            MachineId = notification.MachineId,
            OldStatus = notification.OldStatus,
            NewStatus = notification.NewStatus,
            OccurredOn = DateTimeOffset.UtcNow
        }, ct);
    }
}
```

### Java - Spring Event + Listener
```java
public record MachineStatusChangedEvent(
        UUID machineId,
        MachineStatus oldStatus,
        MachineStatus newStatus,
        OffsetDateTime occurredOn
) {
}

@Component
public class MachineStatusChangedEventListener {

    private final MachineStatusAuditRepository auditRepository;

    public MachineStatusChangedEventListener(MachineStatusAuditRepository auditRepository) {
        this.auditRepository = auditRepository;
    }

    @EventListener
    public void handle(MachineStatusChangedEvent event) {
        MachineStatusAudit audit = MachineStatusAudit.create(
                event.machineId(),
                event.oldStatus(),
                event.newStatus(),
                event.occurredOn()
        );

        auditRepository.save(audit);
    }
}
```

### Comparison
- **.NET** uses explicit domain event abstractions + MediatR notifications.
- **Java** uses Spring’s built-in application event mechanism.
- The .NET solution is more explicit and decoupled from the framework.
- The Java solution is more integrated into the framework and simpler to wire.

## 16.8 Controller Layer
### .NET - Minimal API
```csharp
app.MapPost("/api/machines", async (CreateMachineCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(cmd, ct);
    return Results.Created($"/api/machines/{id}", new { id });
});
```

### Java - Spring REST Controller
```java
@PostMapping
public ResponseEntity<Map<String, UUID>> createMachine(@Valid @RequestBody CreateMachineRequest request) {
    UUID id = machineApplicationService.createMachine(
            new CreateMachineCommand(request.name(), request.pricePerCycle())
    );

    return ResponseEntity
            .created(URI.create("/api/machines/" + id))
            .body(Map.of("id", id));
}
```

### Comparison
- **.NET Minimal APIs** are very concise and lightweight.
- **Spring controllers** are more verbose but familiar to enterprise Java teams.
- Minimal APIs reduce ceremony.
- Spring controllers provide a very clear and conventional MVC structure.

---

## 👤 Author

Patrick Djomo
Software Engineer | Backend Developer