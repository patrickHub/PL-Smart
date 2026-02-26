using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

public class DomainEventsMachineRunningTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DomainEventsMachineRunningTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    private sealed class MachineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public decimal PricePerCycle { get; set; }
        public string Status { get; set; } = "";
    }

    [Fact]
    public async Task CreatingActiveBooking_Should_SetMachineStatusToRunning()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer DE Running", pricePerCycle = 6m });
        createMachine.StatusCode.Should().Be(HttpStatusCode.Created);
        var machine = await createMachine.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = machine!.Id;

        // Create booking active now
        var start = DateTimeOffset.UtcNow;
        var end = DateTimeOffset.UtcNow.AddMinutes(30);

        var createBooking = await _client.PostAsJsonAsync("/api/bookings", new
        {
            machineId,
            startTime = start,
            endTime = end,
            customerName = "DE Customer"
        });

        createBooking.StatusCode.Should().Be(HttpStatusCode.Created);

        // Machine should be Running (domain event handler executed after SaveChanges)
        var getMachine = await _client.GetAsync($"/api/machines/{machineId}");
        getMachine.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await getMachine.Content.ReadFromJsonAsync<MachineDto>();
        dto.Should().NotBeNull();
        dto!.Status.Should().Be("Running");
    }
}