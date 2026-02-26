using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class MachineAuditsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    private readonly ITestOutputHelper _output;

    public MachineAuditsApiTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    private sealed class AuditDto
    {
        public Guid Id { get; set; }
        public Guid MachineId { get; set; }
        public string OldStatus { get; set; } = "";
        public string NewStatus { get; set; } = "";
        public DateTimeOffset OccurredOn { get; set; }
    }

    [Fact]
    public async Task GetMachineAudits_Should_ReturnAuditEntries()
    {
        // Create machine
        var create = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer Audits API", pricePerCycle = 6m });
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var machine = await create.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = machine!.Id;

        // Change status -> creates audit entry
        var statusRes = await _client.PostAsJsonAsync($"/api/machines/{machineId}/status", new { status = "OutOfOrder" });
        statusRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Fetch audits
        var res = await _client.GetAsync($"/api/machines/{machineId}/audits");
        var body = await res.Content.ReadAsStringAsync();

         _output.WriteLine($"Created Booking JSON: {body}");
        
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var audits = await res.Content.ReadFromJsonAsync<List<AuditDto>>();
        audits.Should().NotBeNull();
        audits!.Count.Should().BeGreaterThan(0);
        audits[0].MachineId.Should().Be(machineId);
    }
}