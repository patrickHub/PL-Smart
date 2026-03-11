using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;
using Xunit;
using Xunit.Abstractions;

public class MachineStatusAuditIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    private readonly ITestOutputHelper _output;

    public MachineStatusAuditIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    [Fact]
    public async Task SetMachineStatus_Should_CreateAuditRecord()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer Audit", pricePerCycle = 6m });
        createMachine.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createMachine.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = created!.Id;

        // Change status (this triggers MachineStatusChangedDomainEvent)
        var res = await _client.PostAsJsonAsync($"/api/machines/{machineId}/status", new { status = "OutOfOrder" });
        res.StatusCode.Should().Be(HttpStatusCode.NoContent);
    
        // Assert audit exists (side effect)
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PristaneLaverieSmartDbContext>();

        var audits = db.MachineStatusAudits.Where(a => a.MachineId == machineId).ToList();
        audits.Should().HaveCount(1);
        audits[0].OldStatus.Should().Be(MachineStatus.Available);
        audits[0].NewStatus.Should().Be(MachineStatus.OutOfOrder);
    }
}