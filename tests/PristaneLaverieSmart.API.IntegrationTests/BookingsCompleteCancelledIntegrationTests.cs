using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PristaneLaverieSmart.API.Contracts;
using Xunit;

public class BookingsCompleteCancelledIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingsCompleteCancelledIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    [Fact]
    public async Task CompleteCancelledBooking_Should_Return409()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer CC", pricePerCycle = 6m });
        createMachine.StatusCode.Should().Be(HttpStatusCode.Created);
        var machine = await createMachine.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = machine!.Id;

        // Create booking
        var start = DateTimeOffset.UtcNow.AddHours(2);
        var end = DateTimeOffset.UtcNow.AddHours(3);

        var createBooking = await _client.PostAsJsonAsync("/api/bookings", new
        {
            machineId,
            startTime = start,
            endTime = end,
            customerName = "Customer CC"
        });

        createBooking.StatusCode.Should().Be(HttpStatusCode.Created);
        var booking = await createBooking.Content.ReadFromJsonAsync<CreateResponse>();
        var bookingId = booking!.Id;

        // Cancel booking
        var cancel = await _client.PostAsync($"/api/bookings/{bookingId}/cancel", content: null);
        cancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Complete cancelled -> 409
        var complete = await _client.PostAsync($"/api/bookings/{bookingId}/complete", content: null);
        complete.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await complete.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Status.Should().Be(409);
    }
}