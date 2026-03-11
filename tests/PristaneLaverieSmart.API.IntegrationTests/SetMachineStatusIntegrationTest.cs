using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PristaneLaverieSmart.API.Contracts;
using PristaneLaverieSmart.Domain.Enums;
using Xunit;

public class MachineStatusIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MachineStatusIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    [Fact]
    public async Task SetStatus_Should_Return409_WhenActiveBookingExists()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer Status", pricePerCycle = 6m });
        createMachine.StatusCode.Should().Be(HttpStatusCode.Created);
        var machine = await createMachine.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = machine!.Id;

        // Create an "active now" booking: start 5 min ago, end in 55 min
        var createBooking = await _client.PostAsJsonAsync("/api/bookings", new
        {
            machineId,
            startTime = DateTimeOffset.UtcNow,
            endTime = DateTimeOffset.UtcNow.AddMinutes(60),
            customerName = "Active Customer"
        });

        // If your validator disallows start in the past, change to: start now, end +60 and
        // adjust HasActiveBookingNowAsync to consider start <= now (it will still be active).
        createBooking.StatusCode.Should().Be(HttpStatusCode.Created);

        // Try set machine OutOfOrder while booking active
        var res = await _client.PostAsJsonAsync($"/api/machines/{machineId}/status", new { status = MachineStatus.OutOfOrder });

        res.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await res.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Status.Should().Be(409);
        error.Detail.Should().Contain("active booking");
    }
}