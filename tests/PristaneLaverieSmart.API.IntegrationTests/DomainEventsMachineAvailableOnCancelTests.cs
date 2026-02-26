using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

public class DomainEventsMachineAvailableOnCancelTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DomainEventsMachineAvailableOnCancelTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    private sealed class MachineDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "";
    }

    [Fact]
    public async Task CancellingActiveBooking_Should_SetMachineStatusToAvailable()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer DE Cancel", pricePerCycle = 6m });
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
            customerName = "DE Cancel Customer"
        });

        createBooking.StatusCode.Should().Be(HttpStatusCode.Created);
        var booking = await createBooking.Content.ReadFromJsonAsync<CreateResponse>();
        var bookingId = booking!.Id;

        // Cancel booking
        var cancel = await _client.PostAsync($"/api/bookings/{bookingId}/cancel", content: null);
        cancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Machine should become Available (if no other active booking)
        var getMachine = await _client.GetAsync($"/api/machines/{machineId}");
        getMachine.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await getMachine.Content.ReadFromJsonAsync<MachineDto>();
        dto!.Status.Should().Be("Available");
    }
}